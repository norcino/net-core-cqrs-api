﻿using System.Threading.Tasks;
using Common.Validation;

namespace Service.Transaction.Validator
{
    public class TransactionValidator : IValidator<Data.Entity.Transaction>
    {
        public async Task<IValidationResult> ValidateAsync(Data.Entity.Transaction subject)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrWhiteSpace(subject.Description))
            {
                validationResult.ValidationEntries.Add(new ValidationEntry(nameof(subject.Description), "{0} is mandatory"));
            }

            if (subject.Credit == 0 && subject.Debit == 0)
            {
                validationResult.ValidationEntries.Add(new ValidationEntry(nameof(Data.Entity.Transaction), "{0} must have either a Debit or Credit value"));
            }

            if (subject.CategoryId == 0)
            {
                validationResult.ValidationEntries.Add(new ValidationEntry(nameof(subject.CategoryId), "{0} is mandatory"));
            }
            
            return await Task.FromResult(validationResult);
        }
    }
}
