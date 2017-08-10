namespace Service.Common
{
    public interface ICommand
    {
        //TODO LogInfo ToLog();
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