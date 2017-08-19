using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Service.Common;
using Service.Common.QueryHandlerDecorators;
using Microsoft.Extensions.DependencyModel;

namespace Common.IoC
{
    public class IocConfig
    {
        public static void RegisterContext(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<HouseKeeperContext>(options => options.UseSqlServer(connectionString));

            services.AddSingleton<IHouseKeeperContext>(service => service.GetService<HouseKeeperContext>());
        }

        public static void RegisterServiceManager(IServiceCollection services)
        {
            services.AddSingleton<IServiceManager>(service => new ServiceManager(service));
        }

        public static void RegisterQueryHandlers(IServiceCollection services)
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

                    DecorateHandlerdescriptors(services, interfaceType, typeof(TransactionalQueryHandlerDecorator<,>));
                    DecorateHandlerdescriptors(services, interfaceType, typeof(ExceptionQueryHandlerDecorator<,>));
                    DecorateHandlerdescriptors(services, interfaceType, typeof(LoggingQueryHandlerDecorator<,>));
                }
            }
        }

        public static void RegisterCommandHandlers(IServiceCollection services)
        {
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

//                    DecorateHandlerdescriptors(services, interfaceType, typeof(TransactionalQueryHandlerDecorator<,>));
//                    DecorateHandlerdescriptors(services, interfaceType, typeof(ExceptionQueryHandlerDecorator<,>));
//                    DecorateHandlerdescriptors(services, interfaceType, typeof(LoggingQueryHandlerDecorator<,>));
                }
            }
        }

        private static void DecorateHandlerdescriptors(IServiceCollection services, Type interfaceType,
                Type genericDecoratorType)
            {
                foreach (var descriptor in services.GetDescriptors(interfaceType))
                {
                    object Factory(IServiceProvider serviceProvider)
                    {
                        // Get the instance of the previous decorator
                        var handler = descriptor.ImplementationType != null
                            ? serviceProvider.GetService(descriptor.ImplementationType)
                            : // Used when decorating the handler the first time
                            descriptor.ImplementationFactory(serviceProvider); // Used when decorating another decorator

                        // Create the decorator type including generic types
                        var decoratorType = genericDecoratorType.MakeGenericType(interfaceType.GetGenericArguments());

                        // Create the logger type
                        var loggerType = typeof(ILogger<>).MakeGenericType(decoratorType);

                        return Activator.CreateInstance(decoratorType, handler, serviceProvider.GetService(loggerType));
                    }

                    services.Replace(ServiceDescriptor.Describe(descriptor.ServiceType, Factory,
                        ServiceLifetime.Transient));
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
