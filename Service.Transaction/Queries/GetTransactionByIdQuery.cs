using Common.Log;
using Service.Common;

namespace Service.Transaction.Queries
{
    public class GetTransactionByIdQuery : IQuery<Data.Entity.Transaction>
    {
        public int TransactionId { get; }

        public GetTransactionByIdQuery(int transactionId)
        {
            TransactionId = transactionId;
        }
        
        public LogInfo ToLog()
        {
            const string template = "TransactionId: {TransactionId}";
            return new LogInfo(template, TransactionId);
        }
    }
}
