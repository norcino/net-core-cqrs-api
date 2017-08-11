using Common.Log;

namespace Service.Common
{
    public interface IQuery<TResult>
    {
        LogInfo ToLog();
    }

    public interface IQueryWithTransactionIsolationLevelOverride<TResult> : IQuery<TResult>
    {
        //IsolationLevel? IsolationLevel { get; set; }
    }

    public interface IQueryWithTransactionScopeOptionOverride<TResult> : IQuery<TResult>
    {
        //TransactionScopeOption? TransactionScopeOption { get; set; }
    }
}
