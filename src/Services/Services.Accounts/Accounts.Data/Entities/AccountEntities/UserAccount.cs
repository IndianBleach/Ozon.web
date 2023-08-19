using Accounts.Data.Entities.RoleEntities;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Data.Entities.AccountEntities
{
    public class UserAccount
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser? User { get; set; }

        public string PasswordHash { get; set; }

        public string UserName { get; set; }

        public DateTime DateCreated { get; set; }

        public string RoleId { get; set; }

        public UserRole? Role { get; set; }

        public UserAccount(
            string userId,
            string passwordHash, 
            string userName,
            string roleId)
        {
            RoleId = roleId;
            UserId = userId;
            PasswordHash = passwordHash;
            UserName = userName;
            DateCreated = DateTime.Now;
        }
    }
}
