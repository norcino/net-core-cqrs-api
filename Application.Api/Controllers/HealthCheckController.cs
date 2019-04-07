﻿using System;
using System.Reflection;
using Common.IoC;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Service.Common;

namespace Application.Api.Controllers
{
    [Route("api/[controller]")]
    public class HealthCheckController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public HealthCheckController(IMediator mediator, IHostingEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _mediator = mediator;
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
