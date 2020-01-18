using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Common.Validation;
using Data.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Service.Common;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging.Console;
using Service.Common.CommandAttributes;
using Service.Common.CommandHandlerDecorators;
using Service.Common.QueryHandlerDecorators;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace Common.IoC
{
    /// <summary>
    /// Core Dependency Injection configuration class, contains methods necessary to setup the framework
    /// </summary>
    public static class DependencyInjectionConfiguration
    {
        private static readonly LoggerFactory LoggerFactory =
            new LoggerFactory(new[] {new ConsoleLoggerProvider((_, __) => true, true)});

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
            }
            catch
            {
                LoggerFactory.CreateLogger(typeof(DependencyInjectionConfiguration))
                    ?.LogWarning("Missing or invalid configuration: DatabaseType");
                databaseType = DatabaseType.SQLServer;
            }

            if (hostingEnvironment != null && hostingEnvironment.IsProduction())
            {
                if (databaseType == DatabaseType.SQLiteInMemory)
                {
                    throw new ConfigurationErrorsException(
                        $"Cannot use database type {databaseType} for production environment");
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
                    services.AddSingleton<IHouseKeeperContext>(s =>
                    {
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

                        services.AddSingleton<IHouseKeeperContext>(s =>
                        {
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
                        options.UseLoggerFactory(LoggerFactory);
                    }, poolSize: 5);

                    services.AddTransient<IHouseKeeperContext>(service =>
                        services.BuildServiceProvider()
                            .GetService<HouseKeeperContext>());
                    break;
            }
        }

    }
}