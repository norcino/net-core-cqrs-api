using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Category.Command;

namespace Service.Category.IntegrationTests
{
    [TestClass]
    public class UpdateCategoryCommandHandlerTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public async Task Handler_update_new_category_with_the_correct_properties()
        {
            var category = new Data.Entity.Category
            {
                Name = "Test category",
                Description = "Test description",
                Active = true
            };

            await Context.Categories.AddAsync(category);
            await Context.SaveChangesAsync();

            var updateCategory = new Data.Entity.Category
            {
                Id = category.Id,
                Active = !category.Active,
                Name = category.Name + "2",
                Description = category.Description + "2"
            };

            var command = new UpdateCategoryCommand(category.Id, updateCategory);
            var response = await ServiceManager.ProcessCommandAsync<Data.Entity.Category>(command);

            Assert.IsTrue(response.Successful, "The command response is successful");

            var createdCategory = await Context.Categories.SingleAsync(p => p.Id == response.Result.Id);

            Assert.That.HaveSameProperties(createdCategory, category);
        }
    }
}
