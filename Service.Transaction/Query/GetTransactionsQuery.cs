﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Common.Log;
using Service.Common;
using Service.Common.QueryTreats;

namespace Service.Transaction.Query
{
    public class GetTransactionsQuery : IQuery<List<Data.Entity.Transaction>>, ICanTop, ICanSkip, ICanExpand, ICanOrderBy, ICanFilter<Data.Entity.Transaction>
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
        public Expression<Func<Data.Entity.Transaction, bool>> Filter { get; set; }
    }
}
