using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Data.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyModel;
using Service.Category.Handlers;
using Service.Category.Queries;
using Service.Common;
using Service.Common.QueryHandlerDecorators;

namespace Application.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HouseKeeperContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("HouseKeeping")));

            services.AddSingleton<IHouseKeeperContext>((service) => service.GetService<HouseKeeperContext>());

            services.AddSingleton<IServiceManager>((service) => new ServiceManager(service));

            //  RegisterQueryHandlerDecorators(services);
            RegisterQueryHandlers(services);

            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseMvc();
        }

        private void RegisterQueryHandlers(IServiceCollection service)
        {
            var assemblies = DependencyContext.Default.GetDefaultAssemblyNames()
                .Where(a => a.FullName.StartsWith("Service.") && !a.FullName.StartsWith("Service.Common")).ToList();

            foreach (var assemblyName in assemblies)
            {
                var assembly = Assembly.Load(assemblyName);

                var queryHandlers = assembly.GetTypes()
                    .Where(t => t.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType &&
                                                           i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)));

                foreach (var handlerType in queryHandlers)
                {
                    var interfaceType = handlerType.GetInterfaces().FirstOrDefault(i => i.GetTypeInfo().IsGenericType &&
                                                                               i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

                    service.Add(new ServiceDescriptor(interfaceType, handlerType, ServiceLifetime.Transient));
                }
            }
        }
    
        private void RegisterQueryHandlerDecorators(IServiceCollection service)
        {
            var assemblies = DependencyContext.Default.GetDefaultAssemblyNames().Where(a => a.FullName.StartsWith("Service.")).ToList();
            
            foreach (AssemblyName assemblyName in assemblies)
            {
                Assembly assemblyWithQueryHandlers = Assembly.Load(assemblyName);

                // First decorator registered is the first decorator called
//                RegisterQueryHandlerDecoratorsForAssembly(service, assemblyWithQueryHandlers, typeof(LoggingQueryHandlerDecorator<,>), typeof(IQueryHandler<,>));
//                RegisterQueryHandlerDecoratorsForAssembly(service, assemblyWithQueryHandlers, typeof(TransactionalQueryHandlerDecorator<,>), typeof(IQueryHandler<,>));

//                var handlerTypes = (from t in assemblyWithQueryHandlers.GetTypes()
//                    let iHandlers = t.GetInterfaces().SingleOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
//                    where t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract && iHandlers != null
//                    select iHandlers).ToList();
//
//                foreach (var handlerType in handlerTypes)
//                {
//                    service.Add(new ServiceDescriptor(typeof(IQueryHandler<,>), handlerType, ServiceLifetime.Transient));
//                }
            }
        }

        private void RegisterDecoratorType(IServiceCollection service, Type decoratorType, Type handlerType)
        {
            service.AddTransient(decoratorType, decoratorType.MakeGenericType(handlerType.GetGenericArguments()));
        }
        
        private void RegisterQueryHandlerDecoratorsForAssembly(IServiceCollection service, Assembly assemblyWithHandlers, Type decoratorType, Type decoratorTarget, List<Type> requiredAttribute = null)
        {
            // Get all the handlers in the assembly
            var handlerTypes = (from t in assemblyWithHandlers.GetTypes()
                let iHandlers = t.GetInterfaces().SingleOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == decoratorTarget)
                where t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract && !decoratorType.IsAssignableFrom(t) && iHandlers != null
                select iHandlers).ToList();

            if (handlerTypes.Any())
            {
                foreach (var handlerType in handlerTypes)
                {
                    if (requiredAttribute == null || !requiredAttribute.Any())
                    {
                        RegisterDecoratorType(service, decoratorType, handlerType);
                        continue;
                    }

                    // Get the Handler generic types
                    var genericTypeArguments = handlerType.GenericTypeArguments;

                    // Chek if has IQuery as generic type
                    var hasICommandGenericType = genericTypeArguments.Any(g => g.GetInterfaces().Any(i => i == typeof(IQuery<>)));

                    if (hasICommandGenericType)
                    {
                        // Get the command generic type
                        var queryGenericTypeArgument = genericTypeArguments.FirstOrDefault(a => a.GetInterfaces().Any(i => i == typeof(IQuery<>)));
                        if (queryGenericTypeArgument != null)
                        {
                            // Check if has an attribute that need to be excluded
                            if (queryGenericTypeArgument.GetTypeInfo().CustomAttributes.Any(a => requiredAttribute.Any(l => l == a.AttributeType)))
                            {
                                RegisterDecoratorType(service, decoratorType, handlerType);
                            }
                        }
                    }
                }
            }
        }
    }
}
