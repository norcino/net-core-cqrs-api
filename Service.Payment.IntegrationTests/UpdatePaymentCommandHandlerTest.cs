using System;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Payment.Command;

namespace Service.Payment.IntegrationTests
{
    [TestClass]
    public class UpdatePaymentCommandHandlerTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public async Task Handler_update_payment_with_the_correct_properties()
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

            var payment = new Data.Entity.Payment
            {
                CategoryId = category.Id,
                Debit = 100,
                Description = "Test payment",
                Recorded = DateTime.Now
            };

            await Context.Payments.AddAsync(payment);
            await Context.SaveChangesAsync();

            var updatePayment = new Data.Entity.Payment
            {
                Id = payment.Id,
                CategoryId = categoryTwo.Id,
                Debit = 0,
                Credit = 100,
                Description = payment.Description + "2",
                Recorded = DateTime.Now
            };

            var command = new UpdatePaymentCommand(payment.Id, updatePayment);
            var response = await ServiceManager.ProcessCommandAsync<Data.Entity.Payment>(command);

            Assert.IsTrue(response.Successful, "The command response is successful");

            var savedUpdatedPayment = await Context.Payments.AsNoTracking().SingleAsync(p => p.Id == response.Result.Id);

            savedUpdatedPayment.ShouldHaveSameProperties(updatePayment);
        }
    }
}
