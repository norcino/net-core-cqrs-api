using Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public interface IHouseKeeperContext : IDbContext
    {
        DbSet<Category> Categories { get; set; }
        DbSet<Transaction> Transactions { get; set; }
    }
}
