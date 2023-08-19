using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DTOs
{
    public class UserAccountRoleRead
    { 
        public string RoleId { get; }

        public string RoleName { get;}

        public UserAccountRoleRead(
            string roleId,
            string roleName)
        {
            RoleId = roleId;
            RoleName = roleName;
        }
    }

    public class UserAccountReadDto
    {
        public string UserId { get; set; }

        public string AccountId { get; }

        public string UserName { get; }

        public UserAccountRoleRead? Role { get; }

        public UserAccountReadDto(
            string userId,
            string accountId,
            string userName,
            UserAccountRoleRead? role)
        {
            UserId = userId;
            AccountId = accountId;
            UserName = userName;
            Role = role;
        }
    }
}
