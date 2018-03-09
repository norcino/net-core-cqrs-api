using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class HouseKeeperContext : BaseContext<HouseKeeperContext>, IHouseKeeperContext
    {
        public HouseKeeperContext(DbContextOptions options) : base(options)
        { }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            
            // Get all mappings from the current assembly
            var mappingTypes = Assembly.GetAssembly(GetType())
                .GetTypes()
                .Where(t => t.GetInterfaces()
                .Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            // Get the generic Entity method of the ModelBuilder type
            var entityMethod = typeof(ModelBuilder).GetMethods().Single(x => x.Name == "ApplyConfiguration");

            foreach (var mappingType in mappingTypes)
            {
                // Get the type of entity to be mapped
                var genericTypeArg = mappingType.GetInterfaces().Single().GenericTypeArguments.Single();

                // Create the method using the generic type
                var genericEntityMethod = entityMethod.MakeGenericMethod(genericTypeArg);
                
                // Invoke the mapping method
                genericEntityMethod.Invoke(modelBuilder, new [] { Activator.CreateInstance(mappingType) });
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}