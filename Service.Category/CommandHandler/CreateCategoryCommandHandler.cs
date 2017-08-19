using System.Threading.Tasks;
using Data.Context;
using Service.Category.Command;
using Service.Common;

namespace Service.Category.CommandHandler
{
    public class CreateCategoryCommandHandler : ICommandHandler<CreateCategoryCommand>
    {
        private readonly IHouseKeeperContext _context;

        public CreateCategoryCommandHandler(IHouseKeeperContext context)
        {
            _context = context;
        }

        public async Task<ICommandResponse> HandleAsync(CreateCategoryCommand command)
        {
            await _context.Categories.AddAsync(command.Category);
            await _context.SaveChangesAsync();

            return new CommandResponse<int>(command.Category.Id)
            {
                Successful = true
            };
        }
    }
}