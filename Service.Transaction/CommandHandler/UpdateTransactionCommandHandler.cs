using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Service.Common;
using Service.Transaction.Command;

namespace Service.Transaction.CommandHandler
{
    public class UpdateTransactionCommandHandler : ICommandHandler<UpdateTransactionCommand>
    {
        private readonly IHouseKeeperContext _context;

        public UpdateTransactionCommandHandler(IHouseKeeperContext context)
        {
            _context = context;
        }

        public async Task<ICommandResponse> HandleAsync(UpdateTransactionCommand command)
        {
            var transaction = await _context.Transactions.SingleOrDefaultAsync(p => p.Id == command.TransactionId);

            transaction.Description = command.Transaction.Description;
            transaction.CategoryId = command.Transaction.CategoryId;
            transaction.Credit = command.Transaction.Credit;
            transaction.Debit = command.Transaction.Debit;
            transaction.Recorded = command.Transaction.Recorded;
            
            await _context.SaveChangesAsync();

            return new CommandResponse<Data.Entity.Transaction>(command.Transaction)
            {
                Successful = true
            };
        }
    }
}