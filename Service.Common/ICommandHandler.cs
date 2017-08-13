using System.Threading.Tasks;

namespace Service.Common
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        Task<CommandResponse> HandleAsync(TCommand command);
    }
}