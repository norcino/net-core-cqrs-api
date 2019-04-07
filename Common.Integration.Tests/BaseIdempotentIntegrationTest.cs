using System;
using System.Data;
using Common.IoC;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Common.IntegrationTests
{
    [TestClass]
    public abstract class BaseIdempotentIntegrationTest
    {
        protected IDbContextTransaction Transaction;        
        protected IHouseKeeperContext _context;
        protected IHouseKeeperContext Context
        {
            get
            {
                if (_context != null) return _context;
                _context = ServiceProvider.GetService<IHouseKeeperContext>();
                return _context;
            }
        }
        protected ServiceProvider ServiceProvider { get; private set; }

        public BaseIdempotentIntegrationTest()
        {
            var serviceCollection = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
            serviceCollection.AddSingleton<IConfiguration>(configuration);

            IocConfig.RegisterContext(serviceCollection, null);
            IocConfig.RegisterServiceManager(serviceCollection);
            IocConfig.RegisterValidators(serviceCollection);
            IocConfig.RegisterQueryHandlers(serviceCollection);
            IocConfig.RegisterCommandHandlers(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Context provider is used by the Persisters
            // This need to be the same used by the idempotent tests or the database will
            // be locked by the transaction
            ContextProvider.ServiceProvider = ServiceProvider;
        }

        [TestInitialize]
        public void Initialize()
        {
            Transaction = Context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Transaction?.Rollback();
        }
    }
}
