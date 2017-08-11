using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Data.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Service.Common;
using Service.Common.QueryHandlerDecorators;

namespace Application.Api
{
    public class Startup
    {
        private ILogger _logger;

        public Startup(IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Startup>();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowCredentials();
            });

            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<HouseKeeperContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("HouseKeeping")));

            services.AddSingleton<IHouseKeeperContext>(service => service.GetService<HouseKeeperContext>());

            services.AddSingleton<IServiceManager>(service => new ServiceManager(service));
            
            // Register all the query handlers with the related decoracors
            RegisterQueryHandlers(services);

            // Add framework services.
            services.AddMvc();
        }

        private static void RegisterQueryHandlers(IServiceCollection services)
        {
            foreach (var assembly in AssembliesWithHandlers)
            {
                var queryHandlers = HandlersImplementingInterfaceInAssembly(assembly, typeof(IQueryHandler<,>));

                foreach (var handlerType in queryHandlers)
                {
                    var interfaceType = GetInterfaceInType(handlerType, typeof(IQueryHandler<,>));

                    // Register the handler by it's interface
                    services.Add(new ServiceDescriptor(interfaceType, handlerType, ServiceLifetime.Transient));

                    // Register the handler by it's own type
                    services.Add(new ServiceDescriptor(handlerType, handlerType, ServiceLifetime.Transient));

                    foreach (var descriptor in services.GetDescriptors(interfaceType))
                    {
                        object Factory(IServiceProvider serviceProvider)
                        {
                            // Get the instance of the handler using the current descriptor
                            var handler = serviceProvider.GetService(descriptor.ImplementationType);

                            // Create the decorator type including generic types
                            var loggingDecoratorType = typeof(LoggingQueryHandlerDecorator<,>).MakeGenericType(interfaceType.GetGenericArguments());

                            // Create the logger type
                            var loggerType = typeof(ILogger<>).MakeGenericType(loggingDecoratorType);

                            return Activator.CreateInstance(loggingDecoratorType, handler, serviceProvider.GetService(loggerType));
                        }

                        services.Replace(ServiceDescriptor.Describe(descriptor.ServiceType, Factory, ServiceLifetime.Transient));
                    }

                    foreach (var descriptor in services.GetDescriptors(interfaceType))
                    {
                        object Factory(IServiceProvider serviceProvider)
                        {
                            // Get the instance of the previous decorator
                            var handler = descriptor.ImplementationFactory(serviceProvider);
                            
                            // Create the decorator type including generic types
                            var transactionDecoratorType = typeof(TransactionalQueryHandlerDecorator<,>).MakeGenericType(interfaceType.GetGenericArguments());

                            return Activator.CreateInstance(transactionDecoratorType, handler);
                        }

                        services.Replace(ServiceDescriptor.Describe(descriptor.ServiceType, Factory, ServiceLifetime.Transient));
                    }
                }
            }
        }

        private static Type GetInterfaceInType(Type handlerType, Type handlerInferface) => 
            handlerType.GetInterfaces().FirstOrDefault(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == handlerInferface);

        private static IEnumerable<Type> HandlersImplementingInterfaceInAssembly(Assembly assembly, Type handlerInferface) => 
            assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType &&
                                                   i.GetGenericTypeDefinition() == handlerInferface));

        private static IEnumerable<Assembly> AssembliesWithHandlers
        {
            get
            {
                var assemblies = DependencyContext.Default.GetDefaultAssemblyNames()
                    .Where(a => a.FullName.StartsWith("Service.") && !a.FullName.StartsWith("Service.Common")).ToList();

                foreach (var assemblyName in assemblies)
                {
                    yield return Assembly.Load(assemblyName);
                }
            }
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static List<ServiceDescriptor> GetDescriptors(this IServiceCollection services, Type serviceType)
        {
            var descriptors = services.Where(service => service.ServiceType == serviceType).ToList();

            if (descriptors.Count == 0)
            {
                throw new InvalidOperationException($"Unable to find registered services for the type '{serviceType.FullName}'");
            }

            return descriptors;
        }
    }
}
