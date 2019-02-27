﻿using Common.Log;
using Service.Common;
using Service.Common.CommandAttributes;

namespace Service.Transaction.Command
{
    [ValidateCommand]
    public class CreateTransactionCommand : ICommand
    {
        public Data.Entity.Transaction Transaction { get; set; }

        public CreateTransactionCommand(Data.Entity.Transaction transaction)
        {
            Transaction = transaction;
        }

        public LogInfo ToLog()
        {
            const string template = "Credit: {Credit} Debit: {Debit} CategoryId: {CategoryId}";
            return new LogInfo(template, Transaction.Credit, Transaction.Debit, Transaction.CategoryId);
        }
    }
}
