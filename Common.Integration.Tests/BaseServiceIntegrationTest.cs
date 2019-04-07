using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Common;

namespace Common.IntegrationTests
{    
    public class BaseServiceIntegrationTest : BaseIdempotentIntegrationTest
    {
        protected IMediator mediator;

        public BaseServiceIntegrationTest() : base()
        {
            mediator = new Mediator(ServiceProvider);
        }
    }
}
