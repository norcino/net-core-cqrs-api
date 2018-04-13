﻿using System.Threading.Tasks;
using Common.Validation;

namespace Service.Category.Validator
{
    public class CategoryValidator : IValidator<Data.Entity.Category>
    {
        public async Task<IValidationResult> ValidateAsync(Data.Entity.Category subject)
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
