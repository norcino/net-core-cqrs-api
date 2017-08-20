using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.Common.Exceptions;

namespace Service.Common.CommandHandlerDecorators
{
    public class ExceptionCommandHandlerDecorator<TCommand> : CommandHandlerDecoratorBase<TCommand>, ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ILogger<ExceptionCommandHandlerDecorator<TCommand>> _logger;

        public ExceptionCommandHandlerDecorator(ICommandHandler<TCommand> commandHandler, ILogger<ExceptionCommandHandlerDecorator<TCommand>> logger) : base(commandHandler)
        {
            _logger = logger;
        }
        
        public async Task<ICommandResponse> HandleAsync(TCommand command)
        {
            try
            {
                return await CommmandHandler.HandleAsync(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new CommandHandlerException<TCommand>("CommandHandlerException: " + command, ex, command);
            }
        }
    }
}