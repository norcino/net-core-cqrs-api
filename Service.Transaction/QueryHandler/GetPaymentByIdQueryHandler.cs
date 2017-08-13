using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Service.Common;
using Service.Payment.Query;

namespace Service.Payment.QueryHandler
{
    public class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, Data.Entity.Payment>
    {
        private readonly IHouseKeeperContext _context;

        public GetPaymentByIdQueryHandler(IHouseKeeperContext context)
        {
            _context = context;
        }

        public Task<Data.Entity.Payment> HandleAsync(GetPaymentByIdQuery query)
        {
            return _context.Payments.SingleOrDefaultAsync(c => c.Id == query.PaymentId);
        }
    }
}
