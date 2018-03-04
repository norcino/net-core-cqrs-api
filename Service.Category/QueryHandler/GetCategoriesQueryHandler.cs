using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Category.Query;
using Service.Common;

namespace Service.Category.QueryHandler
{
    public class GetCategoriesQueryHandler : IQueryHandler<GetCategoriesQuery, List<Data.Entity.Category>>
    {
        private readonly IHouseKeeperContext _context;
        private readonly ILogger<IQueryHandler<GetCategoriesQuery, List<Data.Entity.Category>>> _logger;

        public GetCategoriesQueryHandler(IHouseKeeperContext context, ILogger<IQueryHandler<GetCategoriesQuery, List<Data.Entity.Category>>> logger)
        {
            _context = context;
            _logger = logger;
        }
   
        public Task<List<Data.Entity.Category>> HandleAsync(GetCategoriesQuery query)
        {
            return query.ApplyTo(_context.Categories.AsQueryable()).ToListAsync();
        }
    } 
}
