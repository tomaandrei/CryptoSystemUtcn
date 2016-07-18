using System.Data.Entity;

namespace CryptoSystemDissertation.Models
{
    public class CryptoDbContext : DbContext
    {
      protected override void OnModelCreating(DbModelBuilder modelBuilder)
      {
        Database.SetInitializer<CryptoDbContext>(new CreateDatabaseIfNotExists<CryptoDbContext>());
        base.OnModelCreating(modelBuilder);
      }

        public DbSet<UserAccount> UserAccount { get; set; }
        public DbSet<ImageDetails> ImageDetails { get; set; }
    }
}