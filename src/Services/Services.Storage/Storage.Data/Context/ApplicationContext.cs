using Common.Repositories;
using Microsoft.EntityFrameworkCore;
using Products.Data.Entities.Address;
using Products.Data.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<AddressPoint> AddressPoints { get; set; }

        public DbSet<MarketStorage> MarketStorage { get; set; }

        public DbSet<StorageCell> StorageCells { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> opt)
            : base(opt)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
