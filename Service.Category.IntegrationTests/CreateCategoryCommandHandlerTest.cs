using System.Linq;
using System.Threading.Tasks;
using Common.IntegrationTests;
using Common.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Category.Command;

namespace Service.Category.IntegrationTests
{
    [TestClass]
    public class CreateCategoryCommandHandlerTest : BaseIdempotentIntegrationTest
    {
        [TestMethod]
        public async Task Handler_creates_new_category_with_the_correct_properties()
        {
            var category = new Data.Entity.Category
            {
                Name = "Test category",
                Description = "Test description",
                Active = true
            };

            var command = new CreateCategoryCommand(category);
            var response = await ServiceManager.ProcessCommandAsync<int>(command);

            Assert.IsTrue(response.Successful, "The command response is successful");

            var createdCategory = await Context.Categories.SingleAsync(p => p.Id == response.Result);

            createdCategory.ShouldHaveSameProperties(category, "Id");

            Assert.IsTrue(Context.Categories.Any());
        }
    }
}
