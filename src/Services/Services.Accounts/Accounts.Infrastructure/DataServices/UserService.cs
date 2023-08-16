using Common.DataQueries;
using Infrastructure.DataServices;
using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Infrastructure.DataServices
{
    internal class UserService : IUserService
    {


        public Task<QueryResult<UserReadDto>> FindById(string id)
        {
            throw new NotImplementedException();
        }
    }
}
