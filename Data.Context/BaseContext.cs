using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Data.Context
{
    public class BaseContext<TContext> : DbContext where TContext : DbContext
    {
        protected BaseContext()
        { }

        
        protected BaseContext(DbContextOptions options) : base(options)
        {
        }
        
        public void UseTransaction(DbTransaction transaction)
        {
            Database.UseTransaction(transaction);
        }

//        public void SetEntityState(object entity, EntityState state)
//        {
//            Entry(entity).State = state;
//        }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.HasDefaultSchema(DbSchemaStrings.Dbo);
//            modelBuilder.Configurations.AddFromAssembly(Assembly.GetAssembly(typeof(AccountDataMapping)));
//            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
//        }
    }
}