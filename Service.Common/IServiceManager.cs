using System.Threading.Tasks;

namespace Service.Common
{
    public interface IServiceManager
    {
        Task<TResult> ProcessQueryAsync<TResult>(IQuery<TResult> query);
     
        Task<CommandResponse> ProcessCommandAsync(ICommand command);

        Task<CommandResponse<TResult>> ProcessCommandAsync<TResult>(ICommand command);
    }
}
