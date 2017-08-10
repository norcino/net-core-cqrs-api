using Microsoft.EntityFrameworkCore;

namespace Data.Entity
{
    public interface IHouseKeeperContext
    {
        DbSet<Category> Categories { get; set; }
        DbSet<Transaction> Transactions { get; set; }
    }
}
