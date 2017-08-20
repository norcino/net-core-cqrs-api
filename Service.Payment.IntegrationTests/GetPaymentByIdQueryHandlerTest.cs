using System;
using System.Globalization;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Payment.Query;

namespace Service.Payment.IntegrationTests
{
    [TestClass]
    public class GetPaymentByIdQueryHandlerTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public async Task Handler_get_payment_by_id_with_the_correct_properties()
        {
            var category = new Data.Entity.Category
            {
                Name = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Description = "Test description",
                Active = true
            };

            await Context.Categories.AddAsync(category);
            await Context.SaveChangesAsync();

            var payment = new Data.Entity.Payment
            {
                Description = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Recorded = DateTime.Now,
                CategoryId = category.Id
            };

            await Context.Payments.AddAsync(payment);
            await Context.SaveChangesAsync();

            var query = new GetPaymentByIdQuery(payment.Id);
            var response = await ServiceManager.ProcessQueryAsync(query);

            Assert.IsNotNull(response);

            response.ShouldHaveSameProperties(payment);
        }
    }
}
