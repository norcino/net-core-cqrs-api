using Common.Log;
using Service.Common;

namespace Service.Payment.Command
{
    public class CreatePaymentCommand : ICommand
    {
        public Data.Entity.Payment Payment { get; set; }

        public CreatePaymentCommand(Data.Entity.Payment payment)
        {
            Payment = payment;
        }

        public LogInfo ToLog()
        {
            const string template = "Credit: {Credit} Debit: {Debit} CategoryId: {CategoryId}";
            return new LogInfo(template, Payment.Credit, Payment.Debit, Payment.CategoryId);
        }
    }
}
