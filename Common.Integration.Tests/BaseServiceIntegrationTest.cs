using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Common;

namespace Common.IntegrationTests
{    
    public class BaseServiceIntegrationTest : BaseIdempotentIntegrationTest
    {
        protected IServiceManager ServiceManager;

        public BaseServiceIntegrationTest() : base()
        {
            ServiceManager = new ServiceManager(ServiceProvider);
        }
    }
}
