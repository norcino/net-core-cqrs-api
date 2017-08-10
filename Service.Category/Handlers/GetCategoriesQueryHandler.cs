using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entity;
using Microsoft.EntityFrameworkCore;
using Service.Category.Queries;
using Service.Common;

namespace Service.Category.Handlers
{
    public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<Data.Entity.Category>>
    {
        private readonly IHouseKeeperContext _context;

        public GetCategoriesQueryHandler(IHouseKeeperContext context)
        {
            _context = context;
        }

        public Task<List<Data.Entity.Category>> HandleAsync(GetCategoriesQuery query)
        {
            return _context.Categories.ToListAsync();
        }
    }
}
