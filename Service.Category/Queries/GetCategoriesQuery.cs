using System.Collections.Generic;
using Common.Log;
using Service.Common;

namespace Service.Category.Queries
{
    public class GetCategoriesQuery : IQuery<List<Data.Entity.Category>>
    {
        public LogInfo ToLog()
        {
            return new LogInfo();
        }
    }
}
