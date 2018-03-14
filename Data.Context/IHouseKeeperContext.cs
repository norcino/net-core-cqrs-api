using Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public interface IHouseKeeperContext : IDbContext
    {
        DbSet<Category> Categories { get; set; }
        DbSet<Transaction> Transactions { get; set; }
        DbSet<T> Set<T>() where T : class, new();
    }
}
