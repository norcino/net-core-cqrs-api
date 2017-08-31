using System.Linq;
using System.Threading.Tasks;
using Common.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Category.Command;
using Service.Category.Validator;

namespace Service.Category.Tests
{
    [TestClass]
    public class CreateCategoryCommandValidatorTest : CategoryValidatorTest
    {
        private CreateCategoryCommandValidator GetCommandValidator()
        {
            return new CreateCategoryCommandValidator(new CategoryValidator());
        }

        [TestMethod]
        public async Task Validation_suceed_when_category_is_valid_for_creation()
        {
            var category = new Data.Entity.Category { Name = "Category name" };
            var command = new CreateCategoryCommand(category);
            var result = await GetCommandValidator().ValidateAsync(command);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
            Assert.IsFalse(result.ValidationEntries.Any());
        }

        [TestMethod]
        public async Task Validation_fails_when_category_in_command_is_null()
        {
            var command = new CreateCategoryCommand(null);
            var result = await GetCommandValidator().ValidateAsync(command);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ValidationEntries.Any(e =>
                e.PropertyName == nameof(command.Category) &&
                e.Severity == Severity.Error &&
                e.ErrorMessage == "{0} is mandatory"));
        }

        [TestMethod]
        public async Task Validation_fails_when_category_in_command_has_id()
        {
            var category = new Data.Entity.Category { Name = "Category name", Id = 1 };
            var command = new CreateCategoryCommand(category);
            var result = await GetCommandValidator().ValidateAsync(command);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ValidationEntries.Any(e =>
                e.PropertyName == nameof(command.Category.Id) &&
                e.Severity == Severity.Error &&
                e.ErrorMessage == "{0} should not be provided"));
        }
    }
}
