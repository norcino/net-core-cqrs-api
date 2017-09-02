using System;
using System.Data;
using Common.IoC;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Common;

namespace Common.IntegrationTests
{
    [TestClass]
    public abstract class BaseIdempotentIntegrationTest
    {
        protected IDbContextTransaction Transaction;
        protected IHouseKeeperContext Context;
        protected IServiceManager ServiceManager;
        protected bool UseInMemoryDatabase = true;

        [TestInitialize]
        public void Initialize()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder();

            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            if (UseInMemoryDatabase)
            {
                serviceCollection.AddDbContext<HouseKeeperContext>(options =>
                {
                    options.UseInMemoryDatabase(Guid.NewGuid().ToString());
                    options.ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                });
                serviceCollection.AddTransient<IHouseKeeperContext>(service => service.GetService<HouseKeeperContext>());
            }
            else
            {
                IocConfig.RegisterContext(serviceCollection, configuration.GetConnectionString("HouseKeeping_Test"));
            }
            
            IocConfig.RegisterServiceManager(serviceCollection);
            IocConfig.RegisterValidators(serviceCollection);
            IocConfig.RegisterQueryHandlers(serviceCollection);
            IocConfig.RegisterCommandHandlers(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            Context = serviceProvider.GetService<IHouseKeeperContext>();
            Transaction = Context.Database.BeginTransaction(IsolationLevel.ReadCommitted);

            ServiceManager = new ServiceManager(serviceProvider);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Transaction?.Rollback();
        }
    }
}
