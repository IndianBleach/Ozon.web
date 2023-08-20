using Accounts.Data.Entities.AccountEntities;
using Common.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.Specifications
{
    public class AccountByIdSpec : BaseSpecification<UserAccount>
    {
        public AccountByIdSpec(string accountId) : base(x => x.Id.Equals(accountId))
        {
            AddInclude(x => x.Role);
        }
    }
}
