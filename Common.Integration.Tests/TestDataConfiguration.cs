using Common.IoC;
using Data.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.IntegrationTests
{
    public class TestDataConfiguration
    {
        private static bool _initialised = false;
        private static ServiceProvider _serviceProvider;

        public static IHouseKeeperContext GetContext()
        {
            if (!_initialised)
            {
                var serviceCollection = new ServiceCollection();
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();

                var useInMemoryDb = configuration?.GetValue<bool>("UseInMemoryDatabase") ?? true;
                var connectionString = configuration?.GetConnectionString("DB") ?? "";

                IocConfig.RegisterContext(serviceCollection, useInMemoryDb ? "" : connectionString, null);

                _serviceProvider = serviceCollection.BuildServiceProvider();
                _initialised = true;
            }
            
            return _serviceProvider.GetService<IHouseKeeperContext>();
        }
    }
}