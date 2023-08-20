using Accounts.Data.Entities.AccountEntities;
using Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.Specifications
{
    public class AccountByNameSpec : BaseSpecification<UserAccount>
    {
        public AccountByNameSpec(string userName) 
            : base(x => x.UserName.Equals(userName))
        {
            AddInclude(x => x.Role);
        }
    }
}
