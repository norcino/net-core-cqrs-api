using System;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Transaction.Command;

namespace Service.Transaction.IntegrationTests
{
    [TestClass]
    public class UpdateTransactionCommandHandlerTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public async Task Handler_update_transaction_with_the_correct_properties()
        {
            var category = new Data.Entity.Category
            {
                Active = true,
                Name = "Test category",
                Description = "Description category"
            };
            
            var categoryTwo = new Data.Entity.Category
            {
                Active = true,
                Name = "Test category 2",
                Description = "Description category 2"
            };

            await Context.Categories.AddAsync(category);
            await Context.Categories.AddAsync(categoryTwo);
            await Context.SaveChangesAsync();

            var transaction = new Data.Entity.Transaction
            {
                CategoryId = category.Id,
                Debit = 100,
                Description = "Test transaction",
                Recorded = DateTime.Now
            };

            await Context.Transactions.AddAsync(transaction);
            await Context.SaveChangesAsync();

            var updateTransaction = new Data.Entity.Transaction
            {
                Id = transaction.Id,
                CategoryId = categoryTwo.Id,
                Debit = 0,
                Credit = 100,
                Description = transaction.Description + "2",
                Recorded = DateTime.Now
            };

            var command = new UpdateTransactionCommand(transaction.Id, updateTransaction);
            var response = await ServiceManager.ProcessCommandAsync<Data.Entity.Transaction>(command);

            Assert.IsTrue(response.Successful, "The command response is successful");

            var savedUpdatedTransaction = await Context.Transactions.AsNoTracking().SingleAsync(p => p.Id == response.Result.Id);

            savedUpdatedTransaction.ShouldHaveSameProperties(updateTransaction);
        }
    }
}
