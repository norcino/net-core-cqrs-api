using System.Threading.Tasks;

namespace Service.Common
{
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        CommandResponse Handle(TCommand command);

        Task<CommandResponse> HandleAsync(TCommand command);
    }
}