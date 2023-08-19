using Accounts.Data.Entities.AccountEntities;
using Accounts.Data.Entities.RoleEntities;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Data.Context
{
    public class ApplicationContext : DbContext
    {
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<UserAccount> UserAccounts { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
