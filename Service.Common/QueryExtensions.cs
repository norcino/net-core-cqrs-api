using System.Linq;
using Service.Common.QueryTreats;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Service.Common
{
    public static class QueryExtensions
    {
        public static IQueryable<T> ApplyTo<T>(this IQuery query, IQueryable<T> queryable) where T : class
        {
            if (query is ICanOrderBy o && o?.OrderBy?.Count > 0)
            {
                var orderIndex = 0;
                var dynamicOrderString = $"{o.OrderBy[orderIndex].PropertyName} {o.OrderBy[orderIndex].Order}";
                for (orderIndex++; orderIndex < o.OrderBy.Count; orderIndex++)
                {
                    dynamicOrderString += $", {o.OrderBy[orderIndex].PropertyName} {o.OrderBy[orderIndex].Order}";
                }
                queryable = queryable.OrderBy(dynamicOrderString);
            }

            if (query is ICanSkip s)
            {
                queryable = queryable.Skip(s.Skip);
            }

            if (query is ICanTop t && t.Top.HasValue)
            {
                queryable = queryable.Take(t.Top.Value);
            }

            if (query is ICanExpand e && e.Expand?.Length > 0)
            {
                queryable = e.Expand.Aggregate(queryable, (current, expand) => current.Include(expand));
            }

            return queryable;
        }
    }
}
