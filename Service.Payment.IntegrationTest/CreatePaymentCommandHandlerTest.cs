using System;
using System.Linq;
using System.Threading.Tasks;
using Common.IntegrationTest;
using Common.Test;
using Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Payment.Command;

namespace Service.Payment.IntegrationTest
{
    [TestClass]
    public class CreatePaymentCommandHandlerTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public async Task Handler_creates_new_payment_with_the_correct_properties()
        {
            var category = new Category
            {
                Active = true,
                Name = "Test category",
                Description = "Description category"
            };

            await Context.Categories.AddAsync(category);
            await Context.SaveChangesAsync();

            var payment = new Data.Entity.Payment
            {
                CategoryId = category.Id,
                Debit = 100,
                Description = "Test payment",
                Recorded = DateTime.Now
            };
            
            var command = new CreatePaymentCommand(payment);
            var response = await ServiceManager.ProcessCommandAsync<int>(command);

            Assert.IsTrue(response.Successful, "The command response is successful");

            var createdPayment = await Context.Payments.SingleAsync(p => p.Id == response.Result);

            createdPayment.ShouldHaveSameProperties(payment, "Id");

            Assert.IsTrue(Context.Categories.Any());
        }
    }
}
