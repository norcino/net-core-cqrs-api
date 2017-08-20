using Common.Log;
using Service.Common;

namespace Service.Payment.Command
{
    public class UpdatePaymentCommand : ICommand
    {
        public int PaymentId { get; set; }
        public Data.Entity.Payment Payment { get; set; }

        public UpdatePaymentCommand(int id, Data.Entity.Payment payment)
        {
            PaymentId = id;
            Payment = payment;
        }

        public LogInfo ToLog()
        {
            const string template = "Payment Id: {PaymentId} Credit: {Credit} Debit: {Debit} CategoryId: {CategoryId}";
            return new LogInfo(template, PaymentId, Payment.Credit, Payment.Debit, Payment.CategoryId);
        }
    }
}
