using System.Threading.Tasks;

namespace Service.Common
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task<ICommandResponse> HandleAsync(TCommand command);
    }
}