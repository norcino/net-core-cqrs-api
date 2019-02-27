using System;
using System.Globalization;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests.FluentAssertion;
using Data.Common.Testing.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Transaction.Query;

namespace Service.Transaction.IntegrationTests
{
    [TestClass]
    public class GetTransactionByIdQueryHandlerTest : BaseServiceIntegrationTest
    {
        [TestMethod]
        public async Task Handler_get_transaction_by_id_with_the_correct_properties()
        {
            var category = Persister<Data.Entity.Category>.New().Persist();
            var transaction = Persister<Data.Entity.Transaction>.New()
                .Persist(t => t.CategoryId = category.Id);
                        
            var query = new GetTransactionByIdQuery(transaction.Id);
            var dbTransaction = await ServiceManager.ProcessQueryAsync(query);

            Assert.IsNotNull(dbTransaction);
            Assert.That.This(dbTransaction).HasSameProperties(transaction);
        }
    }
}
