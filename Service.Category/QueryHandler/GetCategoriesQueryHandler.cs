using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Service.Category.Query;
using Service.Common;
using Data.Common;

namespace Service.Category.QueryHandler
{
        public static class DynamicExtentions
        {
            public static object GetPropertyDynamic<T>(this T self, string propertyName) where T : class
            {
                var param = Expression.Parameter(typeof(T), "value");
                var getter = Expression.Property(param, propertyName);
                var boxer = Expression.TypeAs(getter, typeof(object));
                var getPropValue = Expression.Lambda<Func<T, object>>(boxer, param).Compile();
                return getPropValue(self);
            }
        }


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
            var queryable = _context.Categories.AsQueryable();

            queryable = query.ApplyTo(queryable);
            
            _logger.LogDebug($"SQL: {queryable.ToSql()}");

            return queryable.ToListAsync();
        }
    } 
}
