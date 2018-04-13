using System;
using System.Globalization;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Transaction.Query;

namespace Service.Transaction.IntegrationTests
{
    [TestClass]
    public class GetTransactionByIdQueryHandlerTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public async Task Handler_get_transaction_by_id_with_the_correct_properties()
        {
            var category = new Data.Entity.Category
            {
                Name = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Description = "Test description",
                Active = true
            };

            await Context.Categories.AddAsync(category);
            await Context.SaveChangesAsync();

            var transaction = new Data.Entity.Transaction
            {
                Description = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Recorded = DateTime.Now,
                CategoryId = category.Id
            };

            await Context.Transactions.AddAsync(transaction);
            await Context.SaveChangesAsync();

            var query = new GetTransactionByIdQuery(transaction.Id);
            var response = await ServiceManager.ProcessQueryAsync(query);

            Assert.IsNotNull(response);

            Assert.That.HaveSameProperties(response, transaction);
        }
    }
}
