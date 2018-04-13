using System;
using System.Globalization;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Category.Query;

namespace Service.Category.IntegrationTests
{
    [TestClass]
    public class CategoryValidatorTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public async Task Handler_get_category_by_id_with_the_correct_properties()
        {
            var category = new Data.Entity.Category
            {
                Name = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                Description = "Test description",
                Active = true
            };

            await Context.Categories.AddAsync(category);
            await Context.SaveChangesAsync();

            var query = new GetCategoryByIdQuery(category.Id);
            var response = await ServiceManager.ProcessQueryAsync(query);

            Assert.IsNotNull(response);

            Assert.That.HaveSameProperties(response, category);
        }
    }
}
