using System;
using System.Threading.Tasks;

namespace Service.Common
{
    public class ServiceManager : IServiceManager
    {
        private readonly IServiceProvider _services;

        public ServiceManager(IServiceProvider services)
        {
            _services = services;
        }

        /// <summary>
        /// Dynamically constructs the correct IQueryHandler for the given query and executes it's Handle method
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<TResult> ProcessQueryAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _services.GetService(handlerType);
            return handler.HandleAsync((dynamic)query);
        }

        public async Task<ICommandResponse> ProcessCommandAsync(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _services.GetService(handlerType);
            return await handler.HandleAsync((dynamic)command); 
        }

        public async Task<ICommandResponse<TResult>> ProcessCommandAsync<TResult>(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            dynamic handler = _services.GetService(handlerType);

            var result = await handler.HandleAsync((dynamic)command);

            if (result == null || result.GetType() == typeof(CommandResponse<TResult>))
            {
                return result;
            }
            
            var specialisedRommandResponse = (ICommandResponse<TResult>) Activator.CreateInstance(typeof(CommandResponse<TResult>));
            specialisedRommandResponse.ValidationEntries = (result as ICommandResponse).ValidationEntries;
            specialisedRommandResponse.Successful = (result as ICommandResponse).Successful;

            return specialisedRommandResponse;
        }
    }
}
