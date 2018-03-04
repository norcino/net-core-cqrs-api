using System.Collections.Generic;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Service.Category.QueryHandler;
using Service.Common.QueryTreats;

namespace Application.Api.Controllers
{
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Apply to the Query object, the query conditions gathered from the ODataQueryOption
        /// </summary>
        /// <typeparam name="T">Type of the query to decorate</typeparam>
        /// <param name="queryOptions">Query options to apply</param>
        /// <param name="query">Query to decorate with OData queery options</param>
        /// <returns>Query decorated with OData query options</returns>
        public T ApplyODataQueryConditions<T>(ODataQueryOptions queryOptions, T query)
        {
            if (query is ICanCount q && queryOptions?.Count != null)
            {
                q.Count = queryOptions?.Count?.Value ?? false;
            }

            if (query is ICanSkip s && queryOptions?.Skip != null)
            {
                s.Skip = queryOptions?.Skip?.Value ?? 0;
            }

            if (query is ICanTop t && queryOptions?.Top != null)
            {
                t.Top = queryOptions?.Top?.Value;
            }

            if (query is ICanExpand e)
            {
                e.Expand = queryOptions?.SelectExpand?.RawExpand?.Split(',');
            }

            if (query is ICanOrderBy o && queryOptions?.OrderBy != null)
            {
                o.OrderBy = new List<OrderDescriptor>();
                var orderClause = queryOptions.OrderBy.OrderByClause;
                while (orderClause != null)
                {
                    var orderDescriptor = new OrderDescriptor
                    {
                        Order = orderClause.Direction == OrderByDirection.Ascending ? Order.Ascending : Order.Descending,
                        PropertyName = ((EdmNamedElement)((SingleValuePropertyAccessNode)orderClause.Expression).Property).Name
                    };

                    o.OrderBy.Add(orderDescriptor);
                    orderClause = orderClause.ThenBy;
                }
            }
           

            var filterClause = queryOptions?.Filter?.FilterClause;

            return query;
        }
    }
}
