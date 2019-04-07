using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Common.Validation;
using Data.Context;
using Data.Entity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Service.Transaction.Command;
using Service.Transaction.Validator;

namespace Common.IoC
{
    public class IocConfig
    {
        public static readonly LoggerFactory MyLoggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });

        public static void RegisterContext(IServiceCollection services, IHostingEnvironment hostingEnvironment)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();            
            var connectionString = configuration.GetConnectionString(Constants.ConfigConnectionStringName);
            var databaseType = DatabaseType.SQLServer;

            try
            {
                databaseType = configuration?.GetValue<DatabaseType>("DatabaseType") ?? DatabaseType.SQLServer;
            }catch
            {
                MyLoggerFactory.CreateLogger<IocConfig>()?.LogWarning("Missing or invalid configuration: DatabaseType");
                databaseType = DatabaseType.SQLServer;
            }

            if(hostingEnvironment != null && hostingEnvironment.IsProduction())
            {
                if(databaseType == DatabaseType.SQLiteInMemory)
                {
                    throw new ConfigurationErrorsException($"Cannot use database type {databaseType} for production environment");
                }
            }

            switch (databaseType)
            {
                case DatabaseType.SQLiteInMemory:
                    // Use SQLite in memory database for testing
                    services.AddDbContext<HouseKeeperContext>(options =>
                    {
                        options.UseSqlite($"DataSource='file::memory:?cache=shared'");
                    });

                    // Use singleton context when using SQLite in memory if the connection is closed the database is going to be destroyed
                    // so must use a singleton context, open the connection and manually close it when disposing the context
                    services.AddSingleton<IHouseKeeperContext>(s => {
                        var context = s.GetService<HouseKeeperContext>();
                        context.Database.OpenConnection();
                        context.Database.EnsureCreated();
                        return context;
                    });
                    break;
                case DatabaseType.SQLServer:
                default:
                    // Use SQL Server testing configuration
                    if (hostingEnvironment == null || hostingEnvironment.IsTesting())
                    {
                        services.AddDbContext<HouseKeeperContext>(options =>
                        {
                            options.UseSqlServer(connectionString);
                        });

                        services.AddSingleton<IHouseKeeperContext>(s => {
                            var context = s.GetService<HouseKeeperContext>();
                            context.Database.EnsureCreated();
                            return context;
                        });

                        break;
                    }

                    // Use SQL Server production configuration
                    services.AddDbContextPool<HouseKeeperContext>(options =>
                    {
                        // Production setup using SQL Server
                        options.UseSqlServer(connectionString);
                        options.UseLoggerFactory(MyLoggerFactory);
                    }, poolSize: 5);

                    services.AddTransient<IHouseKeeperContext>(service =>
                        services.BuildServiceProvider()
                        .GetService<HouseKeeperContext>());
                    break;            
            }
        }

        public static void RegisterValidators(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // TODO: Make dependency injection setup automatic for validators

            services.AddTransient(typeof(ICommandValidator<CreateCategoryCommand>), typeof(CreateCategoryCommandValidator));
            services.AddTransient(typeof(ICommandValidator<CreateTransactionCommand>), typeof(CreateTransactionCommandValidator));
                        
            services.AddTransient<IValidator<Transaction>>(validator => new TransactionValidator());
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
            // Create the generic interface type that an hypothetical command validator would have, the validators are optional
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
                        // Custom decorator constructor that receives an additional type
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
