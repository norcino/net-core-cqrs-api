using System;

namespace Service.Common.Exceptions
{
    public class CommandHandlerException<TCommand> : Exception where TCommand : ICommand
    {
        public TCommand Command { get; private set; }

        public CommandHandlerException(string message, Exception innerException, TCommand command)
            : base(message, innerException)
        {
            Command = command;
        }
    }
}