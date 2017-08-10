using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.Common.QueryHandlerDecorators
{
    public class LoggingQueryHandlerDecorator<TQuery, TResult> : QueryHandlerDecoratorBase<TQuery, TResult>, IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        //   private readonly ILogger _logger;

        public LoggingQueryHandlerDecorator(IQueryHandler<TQuery, TResult> decoratedQueryHandler)//, ILogger logger)
            : base(decoratedQueryHandler)
        {
            // _logger = logger;
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            const string DurationMS = "DurationMS";

            //            var stopWatch = Stopwatch.StartNew();
            //
            //            _logger.Info(string.Format("{0} Started", query.GetType().Name));
            //
            var response = await DecoratedQueryHandler.HandleAsync(query);
            //            stopWatch.Stop();
            //
            //            var originalLogInfo = query.ToLog();
            //
            //            string formattedTime = string.Format("{0:mm\\:ss\\:fff}", stopWatch.Elapsed);
            //            string template = formattedTime + " {QueryName:l} - " + originalLogInfo.LogMessageTemplate;
            //
            //            var properties = new List<object> { query.GetType().Name };
            //            properties.AddRange(originalLogInfo.LogMessageParameters);
            //
            //            var newLogInfo = new LogInfo(template, properties.ToArray());
            //
            //            var log = new Log(query.GetType().Name, newLogInfo);
            //            log.AddContextProperty(DurationMS, stopWatch.ElapsedMilliseconds);
            //            _logger.Info(log);

            return response;
        }
    }
}