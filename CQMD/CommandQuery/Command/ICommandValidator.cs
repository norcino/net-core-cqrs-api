using System.Threading.Tasks;
using Common.Validation;

namespace Service.Common
{
    public interface ICommandValidator<TCommand> where TCommand : ICommand
    {
        Task<IValidationResult> ValidateAsync(TCommand command);
    }
}