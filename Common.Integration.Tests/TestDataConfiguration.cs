using Common.IoC;
using Data.Context;
using Microsoft.Extensions.DependencyInjection;

namespace Common.IntegrationTests
{
    public class TestDataConfiguration
    {
        public static IHouseKeeperContext GetContex()
        {
            var serviceCollection = new ServiceCollection();
            IocConfig.RegisterContext(serviceCollection, "", null);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider.GetService<IHouseKeeperContext>();
        }
    }
}