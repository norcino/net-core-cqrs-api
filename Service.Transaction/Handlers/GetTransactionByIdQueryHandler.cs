﻿using System.Threading.Tasks;
using Data.Entity;
using Microsoft.EntityFrameworkCore;
using Service.Common;
using Service.Transaction.Queries;

namespace Service.Transaction.Handlers
{
    public class GetTransactionByIdQueryHandler : IQueryHandler<GetTransactionByIdQuery, Data.Entity.Transaction>
    {
        private readonly IHouseKeeperContext _context;

        public GetTransactionByIdQueryHandler(IHouseKeeperContext context)
        {
            _context = context;
        }

        public Task<Data.Entity.Transaction> HandleAsync(GetTransactionByIdQuery query)
        {
            return _context.Transactions.SingleOrDefaultAsync(c => c.Id == query.TransactionId);
        }
    }
}
