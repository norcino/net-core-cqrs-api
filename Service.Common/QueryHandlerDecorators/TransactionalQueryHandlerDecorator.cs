//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//
//namespace Service.Common.QueryHandlerDecorators
//{
//    public class TransactionalQueryHandlerDecorator<TQuery, TResult> : QueryHandlerDecoratorBase<TQuery, TResult>,
//        IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
//    {
//       // private readonly ILogger _logger;
//
//        public TransactionalQueryHandlerDecorator(IQueryHandler<TQuery, TResult> decoratedQueryHandler)//, ILogger logger)
//            : base(decoratedQueryHandler)
//        {
//        //    _logger = logger;
//        }
//
//        public Task<TResult> HandleAsync(TQuery query)
//        {
//            Task<TResult> retVal;
////            var attribute = (TransactionQueryAttribute)TypeDescriptor.GetAttributes(query)[typeof(TransactionQueryAttribute)];
////
////            if (attribute != null)
////            {
////                TransactionScopeOption? transactionScopeOption = TransactionManager.GetTransactionScopeFromObject<IQueryWithTransactionScopeOptionOverride<TResult>>(query) ?? attribute.TransactionScopeOption;
////                IsolationLevel? isolationLevel = TransactionManager.GetIsolationLevelFromObject<IQueryWithTransactionIsolationLevelOverride<TResult>>(query) ?? attribute.IsolationLevel;
////
////                using (var transactionScope = TransactionManager.CreateTransactionScope(transactionScopeOption, isolationLevel))
////                {
////                    TransactionManager.LogTransactionStarting(_logger, query);
////
////                    retVal = _decoratedQueryHandler.Handle(query);
////                    transactionScope.Complete();
////
////                    TransactionManager.LogTransactionComplete(_logger, query);
////                }
////            }
////            else
////            {
//                retVal = DecoratedQueryHandler.HandleAsync(query);
////            }
//
//            return retVal;
//        }
//    }
//}
