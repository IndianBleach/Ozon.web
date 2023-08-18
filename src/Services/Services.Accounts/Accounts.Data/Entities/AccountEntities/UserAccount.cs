using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Data.Entities.AccountEntities
{
    internal class UserAccount
    {
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public string PasswordHash { get; set; }

        public string Login { get; set; }
    }
}
