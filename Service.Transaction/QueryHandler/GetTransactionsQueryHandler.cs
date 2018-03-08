using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Common;
using Service.Transaction.Query;

namespace Service.Transaction.QueryHandler
{
    public class GetTransactionsQueryHandler : IQueryHandler<GetTransactionsQuery, List<Data.Entity.Transaction>>
    {
        private readonly IHouseKeeperContext _context;
        private readonly ILogger<IQueryHandler<GetTransactionsQuery, List<Data.Entity.Transaction>>> _logger;

        public GetTransactionsQueryHandler(IHouseKeeperContext context, ILogger<IQueryHandler<GetTransactionsQuery, List<Data.Entity.Transaction>>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public Task<List<Data.Entity.Transaction>> HandleAsync(GetTransactionsQuery query)
        {
            return query.ApplyTo(_context.Transactions.AsQueryable()).ToListAsync();
        }
    }
}
