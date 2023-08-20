using Accounts.Data.Entities.RoleEntities;
using Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.Specifications.Roles
{
    public class RoleByNameSpec : BaseSpecification<UserRole>
    {
        public RoleByNameSpec(string roleName)
            : base(x => x.Name.Equals(roleName))
        { }
    }
}
