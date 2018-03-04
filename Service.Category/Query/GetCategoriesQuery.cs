using System.Collections.Generic;
using Common.Log;
using Service.Common;
using Service.Common.QueryTreats;

namespace Service.Category.Query
{
    public class GetCategoriesQuery : IQuery<List<Data.Entity.Category>>, ICanTop, ICanSkip, ICanExpand, ICanOrderBy
    {
        public LogInfo ToLog()
        {
            return new LogInfo();
        }

        public int? Top { get; set; }
        public int Skip { get; set; }
        public string[] Expand { get; set; }
        public bool Count { get; set; }
        public IList<OrderDescriptor> OrderBy { get; set; }
    }
}
