using System.Collections.Generic;
using Common.Tests;
using Data.Entity;

namespace Data.Common.Testing.Builder
{
    public class CategoryBuilder : BaseBuilder<CategoryBuilder, Category>
    {
        private bool _active = true;
        private string _description = AnonymousData.String("Description");
        private string _name = AnonymousData.String("Name");
        private int _id = AnonymousData.Int();
        private ICollection<Transaction> _transactions = new List<Transaction>();
        
        public override Category Build()
        {
            var category = new Category
            {
                Active = _active,
                Description = _description,
                Name = _name,
                Id = _id,
                Transactions = _transactions
            };

            return category;
        }
        
        public CategoryBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public CategoryBuilder WithId(int id)
        {
            _id = id;
            return this;
        }

        public CategoryBuilder WithActive(bool active)
        {
            _active = active;
            return this;
        }

        public CategoryBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public CategoryBuilder WithTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
            return this;
        }

        public CategoryBuilder WithTransactions(List<Transaction> transactions)
        {
            _transactions = transactions;
            return this;
        }
    }
}
