using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Common.Validation;
using Data.Context;
using Data.Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Service.Common;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging.Console;
using Service.Category.Command;
using Service.Category.Validator;
using Service.Common.CommandAttributes;
using Service.Common.CommandHandlerDecorators;
using Service.Common.QueryHandlerDecorators;

namespace Common.IoC
{
    public class IocConfig
    {
        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });

        public static void RegisterContext(IServiceCollection services, string connectionString, IHostingEnvironment hostingEnvironment)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddDbContext<HouseKeeperContext>(options =>
            {
                if (hostingEnvironment == null || hostingEnvironment.IsTesting())
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();
                    options.UseSqlite(connection);
//                        options.UseInMemoryDatabase("UniqueInMemDbHouseKeeping");
//                        options.ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
//                    options.UseLoggerFactory(MyLoggerFactory);
                }
                else
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(MyLoggerFactory);
                    
                }
            });

            if (hostingEnvironment == null || hostingEnvironment.IsTesting())
            {
                services.AddTransient<IHouseKeeperContext>(service => service.GetService<HouseKeeperContext>());
            } else {
                services.AddSingleton<IHouseKeeperContext>(service =>
                {
                    var context = service.GetService<HouseKeeperContext>();
                    context.Database.EnsureCreated();
                    return context;
                });
            }
        }

        public static void RegisterValidators(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddTransient(typeof(ICommandValidator<CreateCategoryCommand>), typeof(CreateCategoryCommandValidator));
            services.AddTransient<IValidator<Category>>(validator => new CategoryValidator());
        }

        public static void RegisterServiceManager(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IServiceManager>(service => new ServiceManager(service));
        }

        /// <summary>
        /// Register the query handers and all the decorators
        /// </summary>
        /// <param name="services">Service container for IoC</param>
        public static void RegisterQueryHandlers(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

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

                    DecorateHandlerdescriptors(services, interfaceType, typeof(TransactionalQueryHandlerDecorator<,>));
                    DecorateHandlerdescriptors(services, interfaceType, typeof(ExceptionQueryHandlerDecorator<,>));
                    DecorateHandlerdescriptors(services, interfaceType, typeof(LoggingQueryHandlerDecorator<,>));
                }
            }
        }

        /// <summary>
        /// Register the command handers and all the decorators
        /// </summary>
        /// <param name="services">Service container for IoC</param>
        public static void RegisterCommandHandlers(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            foreach (var assembly in AssembliesWithHandlers)
            {
                var commandHandlers = HandlersImplementingInterfaceInAssembly(assembly, typeof(ICommandHandler<>));

                foreach (var handlerType in commandHandlers)
                {
                    var interfaceType = GetInterfaceInType(handlerType, typeof(ICommandHandler<>));

                    // Register the handler by it's interface
                    services.Add(new ServiceDescriptor(interfaceType, handlerType, ServiceLifetime.Transient));

                    // Register the handler by it's own type
                    services.Add(new ServiceDescriptor(handlerType, handlerType, ServiceLifetime.Transient));

                    DecorateHandlerdescriptors(services, interfaceType, typeof(ValidatingCommandHandlerDecorator<>), typeof(ValidateCommandAttribute), GetValidatorInterfaceType(interfaceType));
                    DecorateHandlerdescriptors(services, interfaceType, typeof(TransactionalCommandHandlerDecorator<>));
                    DecorateHandlerdescriptors(services, interfaceType, typeof(ExceptionCommandHandlerDecorator<>));
                    DecorateHandlerdescriptors(services, interfaceType, typeof(LoggingCommandHandlerDecorator<>));
                }
            }
        }

        private static Type GetValidatorInterfaceType(Type interfaceType)
        {
            // Create the generic interface type that an ipotetical command validator would have, the validators are optional
            return typeof(ICommandValidator<>).MakeGenericType(interfaceType.GetGenericArguments());
        }

        /// <summary>
        /// This method decorates handlers or decorators
        /// </summary>
        /// <param name="services">Service container for IoC</param>
        /// <param name="interfaceType">Type of the ICommandHandler interface including the generic type</param>
        /// <param name="genericDecoratorType">Decorator type to use for the current decoration</param>
        /// <param name="requiredAttributeType">Optional attribute that if provided must be present in the command or query in order to enable the current decoration</param>
        /// <param name="optionalDependencyType">Optional interface type that if provided will be used to create an instance passed as parameter to the current decorator</param>
        private static void DecorateHandlerdescriptors(IServiceCollection services, Type interfaceType, Type genericDecoratorType, Type requiredAttributeType = null, Type optionalDependencyType = null)
        {
            if (requiredAttributeType != null)
            {
                // Target type could be ICommand or IQuery
                var targetType = interfaceType.GetGenericArguments().FirstOrDefault();
                if(targetType == null) return;

                var attribute = TypeDescriptor.GetAttributes(targetType)[requiredAttributeType];

                // If the required attribute is not present, do not register the current decorator
                if (attribute == null) return;
            }

            foreach (var descriptor in services.GetDescriptors(interfaceType))
            {
                object Factory(IServiceProvider serviceProvider)
                {
                    // Get the instance of the previous decorator
                    var handler = descriptor.ImplementationType != null
                        // Used when decorating the handler the first time
                        ? serviceProvider.GetService(descriptor.ImplementationType)
                        // Used when decorating another decorator
                        : descriptor.ImplementationFactory(serviceProvider);

                    // Create the decorator type including generic types
                    var decoratorType = genericDecoratorType.MakeGenericType(interfaceType.GetGenericArguments());

                    // Create the logger type
                    var loggerType = typeof(ILogger<>).MakeGenericType(decoratorType);

                    return optionalDependencyType == null 
                        // Standard decorator and handler constructor
                        ? Activator.CreateInstance(decoratorType, handler, serviceProvider.GetService(loggerType)) 
                        // Custom decorator constroctor that receives an additional type
                        : Activator.CreateInstance(decoratorType, handler, serviceProvider.GetService(loggerType), serviceProvider.GetService(optionalDependencyType));
                }

                services.Replace(ServiceDescriptor.Describe(descriptor.ServiceType, Factory, ServiceLifetime.Transient));
            }
        }

        private static Type GetInterfaceInType(Type handlerType, Type handlerInferface) =>
                handlerType.GetInterfaces()
                    .FirstOrDefault(i => i.GetTypeInfo().IsGenericType &&
                                         i.GetGenericTypeDefinition() == handlerInferface);

        private static IEnumerable<Type> HandlersImplementingInterfaceInAssembly(Assembly assembly,
                Type handlerInferface) =>
                assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType &&
                                                                          i.GetGenericTypeDefinition() ==
                                                                          handlerInferface));

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
}
