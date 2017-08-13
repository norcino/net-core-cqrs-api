using System.Threading.Tasks;
using Data.Context;
using Service.Common;
using Service.Payment.Command;

namespace Service.Payment.CommandHandler
{
    public class CreatePaymentCommandHandler : ICommandHandler<CreatePaymentCommand>
    {
        private readonly IHouseKeeperContext _context;

        public CreatePaymentCommandHandler(IHouseKeeperContext context)
        {
            _context = context;
        }

        public async Task<ICommandResponse> HandleAsync(CreatePaymentCommand command)
        {
            await _context.Payments.AddAsync(command.Payment);
            var result = await _context.SaveChangesAsync();

            return new CommandResponse<int>(result)
            {
                Successful = true
            };
        }
    }
}