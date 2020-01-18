using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Common.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using Service.Common;
using Service.Common.CommandAttributes;
using Service.Common.CommandHandlerDecorators;
using Service.Common.QueryHandlerDecorators;

namespace Common.IoC
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCqmd(this IServiceCollection serviceCollection)
        {
            // Register Mediators
            RegisterMediator(serviceCollection);

            // Register the Validators
            RegisterValidators(serviceCollection);

            // Register all the query handlers with the related decorators
            RegisterQueryHandlers(serviceCollection);

            // Register all the command handlers with the related decorators
            RegisterCommandHandlers(serviceCollection);
        }

        /// <summary>
        /// Register the Validator classes used to validate commands and entities
        /// </summary>
        /// <param name="services">Service container for the Dependency Injection</param>
        public static void RegisterValidators(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            RegisterAllTypesInServiceAssembliesImplementingInterface(services, typeof(ICommandValidator<>));
            RegisterAllTypesInServiceAssembliesImplementingInterface(services, typeof(IValidator<>));
        }

        /// <summary>
        /// Register the Mediator class
        /// </summary>
        /// <param name="services">Service container for the Dependency Injection</param>
        private static void RegisterMediator(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IMediator>(service => new Mediator(service));
        }

        /// <summary>
        /// Register the query handers and all the decorators
        /// </summary>
        /// <param name="services">Service container for the Dependency Injection</param>
        private static void RegisterQueryHandlers(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            foreach (var assembly in ServiceAssemblies)
            {
                var queryHandlers = GetGenericTypesImplementingInterfaceInAssembly(assembly, typeof(IQueryHandler<,>));

                foreach (var handlerType in queryHandlers)
                {
                    var interfaceType = GetGenericInterfacesInType(handlerType, typeof(IQueryHandler<,>));

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
        /// <param name="services">Service container for the Dependency Injection</param>
        private static void RegisterCommandHandlers(IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            foreach (var assembly in ServiceAssemblies)
            {
                var commandHandlers = GetGenericTypesImplementingInterfaceInAssembly(assembly, typeof(ICommandHandler<>));

                foreach (var handlerType in commandHandlers)
                {
                    var interfaceType = GetGenericInterfacesInType(handlerType, typeof(ICommandHandler<>));

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

        /// <summary>
        /// Scan all assemblies of the Service Layer, and registers all Types implementing the Generic Interfaces
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="genericInterface">Generic interface</param>
        private static void RegisterAllTypesInServiceAssembliesImplementingInterface(IServiceCollection services, Type genericInterface)
        {
            foreach (var assembly in ServiceAssemblies)
            {
                var validatorTypes = GetGenericTypesImplementingInterfaceInAssembly(assembly, genericInterface);

                foreach (var validatorType in validatorTypes)
                {
                    var interfaceType = GetGenericInterfacesInType(validatorType, genericInterface);
                    services.Add(new ServiceDescriptor(interfaceType, validatorType, ServiceLifetime.Transient));
                }
            }
        }
        
        public static List<ServiceDescriptor> GetDescriptors(this IServiceCollection services, Type serviceType)
        {
            var descriptors = services.Where(service => service.ServiceType == serviceType).ToList();

            if (descriptors.Count == 0)
            {
                throw new InvalidOperationException($"Unable to find registered services for the type '{serviceType.FullName}'");
            }

            return descriptors;
        }

        private static Type GetValidatorInterfaceType(Type interfaceType)
        {
            // Create the generic interface type that an hypothetical command validator would have, the validators are optional
            return typeof(ICommandValidator<>).MakeGenericType(interfaceType.GetGenericArguments());
        }

        /// <summary>
        /// This method decorates handlers or decorators
        /// </summary>
        /// <param name="services">Service container for the Dependency Injection</param>
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
                if (targetType == null) return;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericInferface"></param>
        /// <returns></returns>
        private static Type GetGenericInterfacesInType(Type type, Type genericInferface) =>
                type.GetInterfaces()
                    .FirstOrDefault(i => i.GetTypeInfo().IsGenericType &&
                                         i.GetGenericTypeDefinition() == genericInferface);

        /// <summary>
        /// Get the Types implementing the generic interface provided
        /// </summary>
        /// <param name="assembly">Assembly to scan</param>
        /// <param name="genericInferface">Generic Interface</param>
        /// <returns>All types implementing the generic interface</returns>
        private static IEnumerable<Type> GetGenericTypesImplementingInterfaceInAssembly(Assembly assembly,
                Type genericInferface) =>
                assembly.GetTypes().Where(t => t.GetInterfaces().Any(i => i.GetTypeInfo().IsGenericType &&
                                                                          i.GetGenericTypeDefinition() ==
                                                                          genericInferface));

        /// <summary>
        /// Scan all assemblies matching the criteria used to locate Commands and Handlers
        /// </summary>
        private static IEnumerable<Assembly> ServiceAssemblies
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
