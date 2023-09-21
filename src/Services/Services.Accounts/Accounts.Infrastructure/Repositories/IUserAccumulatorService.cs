using Common.DataQueries;
using Common.DTOs.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.Repositories
{
    public interface IUserAccumulatorService
    {
        QueryResult<UserClaimsJwtToken> RegisterUserIfNotExists(RegisterUserApiPost model);
    }
}
