using System.Transactions;

namespace Service.Common
{
    public interface ICommand
    {
    }

    public interface ICommand<TResult> : ICommand
    {
        TResult Result { get; }
    }

    public interface ICommandWithTransactionIsolationLevelOverride : ICommand
    {
        IsolationLevel? IsolationLevel { get; set; }
    }

    public interface ICommandWithTransactionScopeOptionOverride : ICommand
    {
        TransactionScopeOption? TransactionScopeOption { get; set; }
    }
}