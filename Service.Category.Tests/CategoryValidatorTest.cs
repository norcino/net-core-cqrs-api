using System.Linq;
using System.Threading.Tasks;
using Common.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Service.Category.Validator;

namespace Service.Category.Tests
{
    [TestClass]
    public class CategoryValidatorTest
    {
        private CategoryValidator GetCategoryValidator()
        {
            return new CategoryValidator();
        }

        [TestMethod]
        public async Task Validation_suceed_when_category_is_valid()
        {
            var category = new Data.Entity.Category { Name = "Category name" };
            var result = await GetCategoryValidator().ValidateAsync(category);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsValid);
            Assert.IsFalse(result.ValidationEntries.Any());
        }

        [TestMethod]
        public async Task Validation_fails_when_name_is_null()
        {
            var category = new Data.Entity.Category();
            var result = await GetCategoryValidator().ValidateAsync(category);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ValidationEntries.Any(e => 
                e.PropertyName == nameof(category.Name) && 
                e.Severity == Severity.Error &&
                e.ErrorMessage == "{0} is mandatory"));
        }

        [TestMethod]
        public async Task Validation_fails_when_name_is_empty()
        {
            var category = new Data.Entity.Category { Name = "" };
            var result = await GetCategoryValidator().ValidateAsync(category);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ValidationEntries.Any(e =>
                e.PropertyName == nameof(category.Name) &&
                e.Severity == Severity.Error &&
                e.ErrorMessage == "{0} is mandatory"));
        }

        [TestMethod]
        public async Task Validation_fails_when_name_is_made_of_spaces()
        {
            var category = new Data.Entity.Category { Name = "    " };
            var result = await GetCategoryValidator().ValidateAsync(category);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.ValidationEntries.Any(e =>
                e.PropertyName == nameof(category.Name) &&
                e.Severity == Severity.Error &&
                e.ErrorMessage == "{0} is mandatory"));
        }
    }
}
