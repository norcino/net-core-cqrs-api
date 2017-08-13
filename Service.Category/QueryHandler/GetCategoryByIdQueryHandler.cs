using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Service.Category.Query;
using Service.Common;

namespace Service.Category.QueryHandler
{
    public class GetCategoryByIdQueryHandler : IQueryHandler<GetCategoryByIdQuery, Data.Entity.Category>
    {
        private readonly IHouseKeeperContext _context;

        public GetCategoryByIdQueryHandler(IHouseKeeperContext context)
        {
            _context = context;
        }

        public Task<Data.Entity.Category> HandleAsync(GetCategoryByIdQuery query)
        {
            return _context.Categories.SingleOrDefaultAsync(c => c.Id == query.CategoryId);
        }
    }
}
