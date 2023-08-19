using Accounts.Data.Entities.AccountEntities;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Data.Entities.RoleEntities
{
    public class UserRole
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<UserAccount> UserAccounts { get; set; }

        public UserRole(string name)
        {
            Name = name;
            UserAccounts = new List<UserAccount>();
        }
    }
}
