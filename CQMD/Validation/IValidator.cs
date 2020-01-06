using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Validation
{
    public interface IValidator<in T> where T : class
    {
        Task<IValidationResult> ValidateAsync(T subject);
    }
}
