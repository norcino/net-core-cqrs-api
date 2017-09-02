using System.Threading.Tasks;

namespace Service.Common
{
    public interface IServiceManager
    {
        Task<TResult> ProcessQueryAsync<TResult>(IQuery<TResult> query);
     
        Task<ICommandResponse> ProcessCommandAsync(ICommand command);

        Task<ICommandResponse<TResult>> ProcessCommandAsync<TResult>(ICommand command);
    }
}
