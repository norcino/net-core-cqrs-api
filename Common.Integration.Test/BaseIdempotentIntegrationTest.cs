using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Common.IoC;
using Data.Context;
using Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Common;
using Service.Payment.Command;
using Microsoft.Extensions.Configuration;

namespace Common.Integration.Test
{
    [TestClass]
    public abstract class BaseIdempotentIntegrationTest
    {
        protected IDbContextTransaction Transaction;
        protected IHouseKeeperContext Context;
        protected IServiceManager ServiceManager;

        [TestInitialize]
        public void Initialize()
        {
            var coll = new ServiceCollection();

            var builder = new ConfigurationBuilder();

            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            IocConfig.RegisterContext(coll, configuration.GetConnectionString("HouseKeeping_Test"));
            IocConfig.RegisterServiceManager(coll);
            IocConfig.RegisterQueryHandlers(coll);
            IocConfig.RegisterCommandHandlers(coll);

            var serviceProvider = coll.BuildServiceProvider();

            Context = serviceProvider.GetService<IHouseKeeperContext>();
            ServiceManager = new ServiceManager(serviceProvider);
            Transaction = Context.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Transaction?.Rollback();
        }
    }
}
