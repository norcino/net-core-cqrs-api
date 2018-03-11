using System;
using Common.IoC;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application.Api
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        public IConfigurationRoot Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            HostingEnvironment = env;
            
            _logger = loggerFactory.CreateLogger<Startup>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"Config/appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddFile(Configuration.GetValue<string>("LogFile"));
            
            if (env.IsDevelopment() || env.IsTesting())
            {
                app.UseDeveloperExceptionPage();
                loggerFactory.AddDebug();
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });
            
            app.UseMvc(routeBuilder =>
            {
                routeBuilder
                    .Expand()
                    .Filter()
                    .OrderBy(QueryOptionSetting.Allowed)
                    .MaxTop(50)
                    .Count();
                routeBuilder.EnableDependencyInjection();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(Configuration);

            // Entity Framework context registration
            IocConfig.RegisterContext(services, Configuration.GetConnectionString(Constants.ConfigConnectionStringName), HostingEnvironment);
            
            // Register service manager
            IocConfig.RegisterServiceManager(services);

            // Register the validators
            IocConfig.RegisterValidators(services);

            // Register all the query handlers with the related decoracors
            IocConfig.RegisterQueryHandlers(services);

            // Register all the command handlers with the related decoracors
            IocConfig.RegisterCommandHandlers(services);

            // Add framework services.
            services.AddOData();
            services.AddMvc()
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
        }
    }
}
