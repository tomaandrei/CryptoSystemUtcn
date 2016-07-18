using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.Migrations;

namespace CryptoSystemDissertation.Models
{
  public class Configuration:DbMigrationsConfiguration<CryptoDbContext>
  {
    public Configuration()
    {
      AutomaticMigrationsEnabled = true;
      AutomaticMigrationDataLossAllowed= true;
    }
  }
}