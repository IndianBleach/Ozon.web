using Microsoft.EntityFrameworkCore;
using Products.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        //public DbSet<ProductPreviewImage> ProductImages { get; set; }

        public DbSet<ProductSeller> ProductSellers { get; set; }

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
