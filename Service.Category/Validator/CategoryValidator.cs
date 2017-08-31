using System.Threading.Tasks;
using Common.Validation;

namespace Service.Category.Validator
{
    public class CategoryValidator : IValidator<Data.Entity.Category>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<IValidationResult> ValidateAsync(Data.Entity.Category subject)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(subject.Name))
            {
                validationResult.ValidationEntries.Add(new ValidationEntry(nameof(subject.Name), "{0} is mandatory"));
            }

            return validationResult;
        }
    }
}
