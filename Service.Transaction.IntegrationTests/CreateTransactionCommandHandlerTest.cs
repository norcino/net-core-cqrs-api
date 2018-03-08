using System;
using System.Linq;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Transaction.Command;

namespace Service.Transaction.IntegrationTests
{
    [TestClass]
    public class CreateTransactionCommandHandlerTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public async Task Handler_creates_new_transaction_with_the_correct_properties()
        {
            var category = new Data.Entity.Category
            {
                Active = true,
                Name = "Test category",
                Description = "Description category"
            };

            await Context.Categories.AddAsync(category);
            await Context.SaveChangesAsync();

            var transaction = new Data.Entity.Transaction
            {
                CategoryId = category.Id,
                Debit = 100,
                Description = "Test transaction",
                Recorded = DateTime.Now
            };
            
            var command = new CreateTransactionCommand(transaction);
            var response = await ServiceManager.ProcessCommandAsync<int>(command);

            Assert.IsTrue(response.Successful, "The command response is successful");

            var createdTransaction = await Context.Transactions.SingleAsync(p => p.Id == response.Result);

            createdTransaction.ShouldHaveSameProperties(transaction, "Id");

            Assert.IsTrue(Context.Categories.Any());
        }
    }
}
