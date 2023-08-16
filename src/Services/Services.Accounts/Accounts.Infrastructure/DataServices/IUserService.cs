using Common.DataQueries;
using Infrastructure.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DataServices
{
    internal interface IUserService
    {
        Task<QueryResult<UserReadDto>> FindById(string id);
    }
}
