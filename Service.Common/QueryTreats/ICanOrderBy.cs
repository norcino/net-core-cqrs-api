using System.Collections.Generic;

namespace Service.Common.QueryTreats
{
    public interface ICanOrderBy
    {
        IList<OrderDescriptor> OrderBy { get; set; }
    }
}