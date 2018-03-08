using System.Threading.Tasks;
using Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class HouseKeeperContext : BaseContext<HouseKeeperContext>, IHouseKeeperContext
    {
        public HouseKeeperContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(250)");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.CategoryId).HasDefaultValueSql("0");

                entity.Property(e => e.Credit).HasColumnType("money");

                entity.Property(e => e.Debit).HasColumnType("money");

                entity.Property(e => e.Description).HasColumnType("varchar(500)");

                entity.Property(e => e.Recorded).HasColumnType("datetime");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Transaction_Categories");
            });
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}