using System.Collections.Generic;

namespace Service.Common.QueryTraits
{
    public interface ICanOrderBy
    {
        IList<OrderDescriptor> OrderBy { get; set; }
    }
}