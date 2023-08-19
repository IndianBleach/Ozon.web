using Common.DataQueries;
using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataServices
{
    public interface IUserAccountService
    {
        Task<QueryResult<UserAccountReadDto>> FindAccountById(string userAccountId);

        Task<QueryResult<UserAccountReadDto>> FindAccountByName(string userName);


    }
}
