using System;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using Service.Common;

namespace Application.Api.Controllers
{
    [Route("api/[controller]")]
    public class HealthCheckController : Controller
    {
        private readonly IServiceManager _serviceManager;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public HealthCheckController(IServiceManager serviceManager, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _serviceManager = serviceManager;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }
        
        [HttpGet]
        public ActionResult Get()
        {
            var db = _configuration.GetConnectionString(Constants.ConfigConnectionStringName);

            if (db != null)
            {
                foreach (var token in db?.Split(';'))
                {
                    if (token.StartsWith("database", StringComparison.InvariantCultureIgnoreCase))
                    {
                        db = token.Replace("database", "", StringComparison.InvariantCultureIgnoreCase)
                            .Replace("=", "")
                            .Replace(";", "")
                            .Trim(' ');
                    }
                }
            }

            dynamic result = new
            {
                Environemnt = _hostingEnvironment.EnvironmentName,
                Version = typeof(Startup).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,
                Database = db
            };
            
            return new OkObjectResult(result);
        }
    }
}
