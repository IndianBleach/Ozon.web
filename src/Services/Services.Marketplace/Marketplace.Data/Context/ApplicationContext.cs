using Marketplace.Data.Entities.Address;
using Marketplace.Data.Entities.CatalogEntities;
using Marketplace.Data.Entities.ProductsEntities;
using Marketplace.Data.Entities.PropertyEntities;
using Marketplace.Data.Entities.Sellers;
using Marketplace.Data.Entities.Storages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Context
{
    public class ApplicationContext : DbContext
    {
        // +sync services.storage
        public DbSet<ProductsStorages> ProductsStorages { get; set; }

        // +sync services.storage
        public DbSet<MarketplaceProductStorage> MarketplaceProductStorages { get; set; }

        // +sync services.productRegistration
        public DbSet<MarketplaceSeller> MarketplaceSellers { get; set; }

        public DbSet<AddressPoint> AddressPoints { get; set; }

        public DbSet<CategoryProperty> CategoryProperties { get; set; }

        public DbSet<SectionProperty> SectionProperties { get; set; }

        public DbSet<CatalogProperty> CatalogProperties { get; set; }

        public DbSet<CatalogPropertyUnit> CatalogPropertyUnits { get; set; }

        public DbSet<ProductPropertyValue> ProductPropertyValues { get; set; }

        public DbSet<CatalogProduct> CatalogProducts { get; set; }

        public DbSet<CategorySection> CategorySections { get; set; }

        public DbSet<CatalogCategory> CatalogCategories { get; set; }

        public DbSet<Catalog> Catalogs { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductsStorages>().HasKey(u => new { u.ProductId, u.InStorageId});

            modelBuilder.Entity<SectionProperty>().HasKey(u => new { u.PropertyId, u.SectionId });

            modelBuilder.Entity<CategoryProperty>().HasKey(u => new { u.PropertyId, u.CategoryId });

            modelBuilder.Entity<ProductPropertyValue>().HasKey(u => new { u.PropertyId, u.CatalogProductId});

            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly());
        }
    }
}
