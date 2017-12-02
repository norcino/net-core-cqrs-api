properties {
	$baseDir = resolve-path .\..\
    $srcDir = Join-Path $baseDir ""
    $solution = Join-Path  $srcDir "HouseKeeper.sln"
	$projectDir = Join-Path $srcDir "\Application.Api"
	$project = Join-Path  $projectDir "\Application.Api.csproj"
	$buildDir = Join-Path  $baseDir "build"
	$publishDir = Join-Path $srcDir "\obj\Docker\publish"
	$imageDestinationDir = ".\DockerImage"
	$dockerfile = Join-Path $projectDir "\Dockerfile"
	$projBaseDir = resolve-path .\..\
    $buildConfiguration = "Release"
	$containerUrl = "tcp://nas.home:2376"
	$containerName = "CoreApiDocker"
	$imageName = "housekeeper"
	$hostname = "housekeeper"
	$containerMacAddress = "02:42:49:58:61:80"
	$containerIpAddress = "192.168.0.139"
	$uiVersionParameter = $uiversion
	$apiVersionParameter = $apiversion
	$dataVolumePath = "/share/HouseKeeper/Data"
	$logsVolumePath = "/share/HouseKeeper/Logs"
	$wwwrootVolumePath = "/share/HouseKeeper/wwwroot"
	$configVolumePath = "/share/HouseKeeper/Config"
}

# invoke-psake .\build.ps1 
# publish deploys API

task default -depends help

# To pass parameters use: 
# invoke-psake publish -parameters @{"apiversion"="1.0.0"}

properties {
  $apiVersionParameter = $apiversion
}

# Builds, create the image, uploads it
task publish -depends check-docker-running, build, generate-image, publish-image, create-container, start-container
task publish-nas -depends check-docker-running-nas, build, generate-image-nas, publish-image-nas, create-container-nas, start-container-nas

# Publish the image removing the existing and adding the new
task publish-image -depends check-docker-running, remove-containers

# Cleans the solution
task clean-all -depends clean-debug, clean-release

task help {
	Exec { echo "" }
	Exec { echo "Example of execution:" }
	Exec { echo "" }
	Exec { echo "invoke-psake .\build.ps1" }
	Exec { echo 'invoke-psake .\build.ps1 publish -parameters @{"apiversion"="1.0.0"}' }
	Exec { echo "" }
	Exec { echo "" }
	Exec { echo "publish:			Publishes the current version of the API"}	
	Exec { echo "publish-image:		Publish the image removing the existing and adding the new"}
	Exec { echo "clean-debug:"}
	Exec { echo "restore:"}
	Exec { echo "get-version:"}
	Exec { echo "increment-api-version:"}
	Exec { echo "generate-image:"}
	Exec { echo "stop-containers:"}
	Exec { echo "generate-image:"}
	Exec { echo "remove-images: "}
	Exec { echo "create-container:"}
	Exec { echo "create-container:"}	
}

#
# ---------------------------------------------------------------------------------------------------------
#

# Sets the new API version in the files
task set-version -depends get-version {
	Log("Setting API version to appsettings.json")
	(Get-Content $projectDir\appsettings.json).replace('$imageName:version}', $script:buildversion) | Set-Content $projectDir\appsettings.json
}

# Clean the debug build result of the solution
task clean-debug {
	Log("Cleaning API debug build")
    Exec { dotnet clean -c Debug $solution }
}

# Clean the release build result of the solution
task clean-release {
	Log("Cleaning API release build")
    Exec { dotnet clean -c Release $solution }
}

# Restores nuget packages
task restore {
	Log("Restoring API source packages")
    Exec { dotnet restore $solution }
}

# Get the API version using the parameter if available otherwhise from the file
task get-version {
	Log("Getting current API version")
	if ($apiVersionParameter) {
		$script:buildversion = $apiVersionParameter
		Set-Content .\app.version $script:buildversion
	} else {
		$script:buildversion = (Get-Content .\app.version)
	}
	
	Exec { "API version " + $script:buildversion }
}

# Increment the current API version
task increment-api-version -depends get-version {
	Log("Increment the current API version ($script:buildversion)")
	$fileVersion = $script:buildversion.Split(".")
	$fileVersion[2] = [int]$fileVersion[2] + 1
	$fileVersion -join "." | Set-Content .\app.version
	$script:buildversion = $fileVersion -join "."
	
	Exec { "New API version " + $script:buildversion }
}

# Generate the solution publishing folder
task publish-solution -depends build {
	Log("Generating publish folder")
    Exec { dotnet publish --output $publishDir $project }
}

# Generate the image using the published content
task generate-image -depends publish-solution, set-version {
	Log("Building docker image")
	Log("- " + $dockerfile)
	Log("- " + $srcDir)
    Exec { docker build -f $dockerfile -t $imageName":"$script:buildversion $srcDir }
}

# Stop all the remote containers using the image
task stop-containers {		
	Log("Stopping existing docker containers")
	Exec { docker ps -a -f ancestor=$containerName --no-trunc -q | foreach-object { docker stop $_ } }
	Exec { docker ps -a -f name=$containerName --no-trunc -q | foreach-object { docker stop $_ } }
}

# Removes all the remote images
task remove-images -depends stop-containers {
	Log("Removing existing docker images")
	Exec { 
		$images = @();
		$imagestoremove = @();

		docker images -a | foreach-object { $data = $_ -split '\s+';
			$image = new-object psobject
			$image | add-member -type noteproperty -Name "Repository" -value $data[0]
			$image | add-member -type noteproperty -Name "Tag" -value $data[1]
			$image | add-member -type noteproperty -Name "Image" -value $data[2]
			$images += $image 
		};

		$imagestoremove = $images | Where-Object { 
			$_.Repository.StartsWith($imageName) -or $_.Tag.StartsWith($imageName) -or $_.Tag -eq "<none>" -or $_.Repository -eq "<none>"
		} | select Image;
		
		$imagestoremove | foreach-object { docker rmi -f $_.Image };
	}
}

# Remove all the remote containers
task remove-containers -depends stop-containers {		
	Log("Removing docker cointainers")
	Exec { 
		docker ps -a -f ancestor=$containerName* --no-trunc -q | foreach-object { docker rm -f $_ }
		docker ps -a -f name=$containerName* --no-trunc -q | foreach-object { docker rm -f $_ }
	}
}

# Create the network used by the container
task create-container-network {
	Exec {
		$existingnetworks = docker --tls -H="$containerUrl" network ls -f 'name=bridged-network'
				
		If ($existingnetworks.count -gt 1) {
			write-host "Network container already exists"
		} Else {
			docker --tls -H="$containerUrl" network create --driver "qnet" -d qnet --ipam-driver=qnet --ipam-opt=iface=eth0 --subnet "192.168.0.0/24" --gateway "192.168.0.1" bridged-network
		}
	}
}

# Start the newly created container
task start-container {
	Exec { 
		docker start $containerName$script:buildversion
	}
}

# Create the container using the latest version of the image
task create-container -depends get-version, create-container-network {
	Exec {
		docker --tls -H="$containerUrl" create --hostname $hostname --name $containerName --workdir '/app' --publish-all=true --volume $script:dataVolumePath":/app/Data:rw" --volume $script:logsVolumePath":/app/Logs:rw" --volume $script:wwwrootVolumePath":/app/wwwroot:rw" --volume $script:configVolumePath":/app/Config:rw" --publish-all=true --net "bridged-network" --ip $script:containerIpAddress --publish "0.0.0.0::80" --mac-address=$script:containerMacAddress -t -i $imageName":"$script:buildversion
	}
}

# Overrides the local docker API version to be compatible with the remote server's version
task force-docker-api-nas {
	Log("Forcing docker client API to version 1.23")
	Exec { $env:DOCKER_API_VERSION = 1.23 }
}

#
# ---------------------------------------------------------------------------------------------------------
#

# Helper function to log in console the list of operations
function Log ($msg)
{
	write-host ""
	write-host "----------------------------------------------------------"
	write-host $msg -foreground "Magenta"
	write-host "----------------------------------------------------------"
	write-host ""
}

# Clean the solution and restores the nuget packages and builds the solution
task build -depends clean-all, restore, set-version {
	Log("Building API application")
    Exec { dotnet build -c $buildConfiguration $solution }
}

# Check that docker is running
task check-docker-running {
	Try
	{
		Exec { docker ps }
	}
	Catch
	{
		Log("Docker is not running, please start it")
		break
	}    
}

task check-docker-running-nas -depends force-docker-api-nas {
	Try
	{
		Exec { docker --tls -H="$containerUrl" ps }
	}
	Catch
	{
		Log("Docker is not running, please start it")
		break
	}    
}