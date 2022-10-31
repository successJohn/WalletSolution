using Microsoft.EntityFrameworkCore;
using WalletSolution.Models;

namespace WalletSolution.Infrastructure.Persistence
{
    public class WalletDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<NGNWallet> NGNWallets { get; set; }
        public DbSet<USDWallet> USDWallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        public WalletDbContext(
            DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => {
                entity.HasIndex(e => e.Email).IsUnique(true);
            });

            modelBuilder.Entity<NGNWallet>(entity =>
            {
                entity.HasIndex(e => e.AccountNumber).IsUnique(true);
            });

            modelBuilder.Entity<USDWallet>(entity =>
            {
                entity.HasIndex(e => e.AccountNumber).IsUnique(true);
            });
        }
    }
}