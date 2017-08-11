using System;

namespace Service.Common.Exceptions
{
    public class QueryHandlerException<TQuery, TResult> : Exception where TQuery : IQuery<TResult>
    {
        public TQuery Query { get; private set; }

        public QueryHandlerException(string message, Exception innerException, TQuery query)
            : base(message, innerException)
        {
            Query = query;
        }
    }
}