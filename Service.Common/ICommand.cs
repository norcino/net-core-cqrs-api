using System;
using Common.Log;

namespace Service.Common
{
    public interface ICommand
    {
        LogInfo ToLog();
    }

    public interface ICommand<TResult> : ICommand
    {
        TResult Result { get; }
    }

    public interface ICommandWithTransactionIsolationLevelOverride : ICommand
    {
        //TODO IsolationLevel? IsolationLevel { get; set; }
    }

    public interface ICommandWithTransactionScopeOptionOverride : ICommand
    {
        //TODO TransactionScopeOption? TransactionScopeOption { get; set; }
    }
}