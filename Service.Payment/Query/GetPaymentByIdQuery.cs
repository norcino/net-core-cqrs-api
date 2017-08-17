using Common.Log;
using Service.Common;

namespace Service.Payment.Query
{
    public class GetPaymentByIdQuery : IQuery<Data.Entity.Payment>
    {
        public int PaymentId { get; }

        public GetPaymentByIdQuery(int paymentId)
        {
            PaymentId = paymentId;
        }
        
        public LogInfo ToLog()
        {
            const string template = "PaymentId: {PaymentId}";
            return new LogInfo(template, PaymentId);
        }
    }
}
