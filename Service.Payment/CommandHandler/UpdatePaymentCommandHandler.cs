using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Service.Common;
using Service.Payment.Command;

namespace Service.Payment.CommandHandler
{
    public class UpdatePaymentCommandHandler : ICommandHandler<UpdatePaymentCommand>
    {
        private readonly IHouseKeeperContext _context;

        public UpdatePaymentCommandHandler(IHouseKeeperContext context)
        {
            _context = context;
        }

        public async Task<ICommandResponse> HandleAsync(UpdatePaymentCommand command)
        {
            var payment = await _context.Payments.SingleOrDefaultAsync(p => p.Id == command.PaymentId);

            payment.Description = command.Payment.Description;
            payment.CategoryId = command.Payment.CategoryId;
            payment.Credit = command.Payment.Credit;
            payment.Debit = command.Payment.Debit;
            payment.Recorded = command.Payment.Recorded;
            
            await _context.SaveChangesAsync();

            return new CommandResponse<Data.Entity.Payment>(command.Payment)
            {
                Successful = true
            };
        }
    }
}