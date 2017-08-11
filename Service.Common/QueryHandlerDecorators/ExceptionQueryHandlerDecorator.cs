using System;
using System.Threading.Tasks;
using Common.Log;
using Microsoft.Extensions.Logging;
using Service.Common.Exceptions;

namespace Service.Common.QueryHandlerDecorators
{
    public class ExceptionQueryHandlerDecorator<TQuery, TResult> : QueryHandlerDecoratorBase<TQuery, TResult>, IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly ILogger<ExceptionQueryHandlerDecorator<TQuery, TResult>> _logger;

        public ExceptionQueryHandlerDecorator(IQueryHandler<TQuery, TResult> queryHandler, ILogger<ExceptionQueryHandlerDecorator<TQuery, TResult>> logger) : base(queryHandler)   
        {
            _logger = logger;
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            try
            {
                return await DecoratedQueryHandler.HandleAsync(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(LogEvent.QueryHandlying, ex, query.ToLog().ToString());

                throw new QueryHandlerException<TQuery, TResult>("QueryHandlerException: " + query, ex, query);
            }
        }
    }
}