using System;
using System.Data;
using Common.IoC;
using Data.Context;
using Microsoft.EntityFrameworkCore;
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
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var useInMemoryDb = configuration?.GetValue<bool>("UseInMemoryDatabase") ?? true;
            var connectionString = configuration?.GetConnectionString("DB") ?? "";

            IocConfig.RegisterContext(serviceCollection, useInMemoryDb ? "" : connectionString, null);
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
