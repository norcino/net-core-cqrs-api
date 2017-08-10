namespace Service.Common
{
    public interface IQuery<TResult>
    {
        //TODO LogInfo ToLog();
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
