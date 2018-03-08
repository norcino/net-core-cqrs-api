using System.Threading.Tasks;
using Data.Context;
using Service.Common;
using Service.Transaction.Command;

namespace Service.Transaction.CommandHandler
{
    public class CreateTransactionCommandHandler : ICommandHandler<CreateTransactionCommand>
    {
        private readonly IHouseKeeperContext _context;

        public CreateTransactionCommandHandler(IHouseKeeperContext context)
        {
            _context = context;
        }

        public async Task<ICommandResponse> HandleAsync(CreateTransactionCommand command)
        {
            await _context.Transactions.AddAsync(command.Transaction);
            await _context.SaveChangesAsync();

            return new CommandResponse<int>(command.Transaction.Id)
            {
                Successful = true
            };
        }
    }
}