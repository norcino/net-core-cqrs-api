using System.Collections.Generic;

namespace Common.Validation
{
    public interface IValidationResult
    {
        bool IsValid { get; }
        IList<ValidationEntry> ValidationEntries { get; }
        IValidationResult Merge(IValidationResult validationResult);
    }
}