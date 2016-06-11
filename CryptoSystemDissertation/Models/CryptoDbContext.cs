using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace CryptoSystemDissertation.Models
{
    public class CryptoDbContext : DbContext
    {
        public DbSet<UserAccount> UserAccount { get; set; }
        public DbSet<ImageDetails> ImageDetails { get; set; }
    }
}