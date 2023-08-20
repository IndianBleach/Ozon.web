using Accounts.Data.Entities.RoleEntities;
using Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.Specifications.Roles
{
    public class RoleByIdSpec : BaseSpecification<UserRole>
    {
        public RoleByIdSpec(string roleId)
            : base(x => x.Id.Equals(roleId))
        {}
    }
}
