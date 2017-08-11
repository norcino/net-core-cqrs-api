using Common.Log;

namespace Service.Common
{
    public interface IQuery<TResult>
    {
        LogInfo ToLog();
    }

    public interface IQueryWithTransactionIsolationLevelOverride<TResult> : IQuery<TResult>
    {
      //TODO  IsolationLevel? IsolationLevel { get; set; }
    }

    public interface IQueryWithTransactionScopeOptionOverride<TResult> : IQuery<TResult>
    {
      //TODO  TransactionScopeOption? TransactionScopeOption { get; set; }
    }
}
