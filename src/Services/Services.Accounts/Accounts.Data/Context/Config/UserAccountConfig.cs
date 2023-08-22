using Accounts.Data.Entities.AccountEntities;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Data.Context.Config
{
    //UserAccount
    public class UserAccountConfig : IEntityTypeConfiguration<UserAccount>
    {
        public void Configure(EntityTypeBuilder<UserAccount> builder)
        {
            builder.HasOne(x => x.Role)
                .WithMany(x => x.UserAccounts)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class UserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasMany(x => x.Accounts)
                .WithOne(x => x.User)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
