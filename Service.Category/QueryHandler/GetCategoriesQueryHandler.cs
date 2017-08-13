using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Service.Category.Query;
using Service.Common;

namespace Service.Category.QueryHandler
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
