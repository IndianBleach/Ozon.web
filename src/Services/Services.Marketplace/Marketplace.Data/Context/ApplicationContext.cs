using Marketplace.Data.Entities.CatalogEntities;
using Marketplace.Data.Entities.ProductsEntities;
using Marketplace.Data.Entities.PropertyEntities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Context
{
    public class ApplicationContext : DbContext
    {
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
        {
            Database.EnsureCreated();
        }
    }
}
