using Common.Repositories;
using Microsoft.EntityFrameworkCore;
using Storage.Data.Entities.Actions;
using Storage.Data.Entities.Address;
using Storage.Data.Entities.Employees;
using Storage.Data.Entities.Products;
using Storage.Data.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Data.Context
{
    public class StorageDbContext : DbContext
    {
        public DbSet<StorageProduct> StorageProducts { get; set; }

        public DbSet<StorageEmployee> StorageEmployees { get; set; }

        public DbSet<AddressPoint> AddressPoints { get; set; }

        public DbSet<MarketStorage> MarketStorage { get; set; }

        public DbSet<StorageCell> StorageCells { get; set; }

        public DbSet<StorageActionType> StorageActionTypes { get; set; }

        public StorageDbContext(DbContextOptions<StorageDbContext> opt)
            : base(opt)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
