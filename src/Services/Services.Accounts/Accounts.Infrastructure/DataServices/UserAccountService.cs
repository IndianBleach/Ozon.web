using Accounts.Data.Entities.AccountEntities;
using Common.DataQueries;
using Data.Entities;
using Infrastructure.DataServices;
using Infrastructure.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Data.Context;

namespace User.Infrastructure.DataServices
{
    public class UserAccountService : IUserAccountService
    {
        private readonly ApplicationContext _dbContext;

        public UserAccountService(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<QueryResult<UserAccountReadDto>> FindAccountById(string id)
        {
            UserAccount? account = await _dbContext.UserAccounts
                .FirstOrDefaultAsync(x => x.Id == id);

            if (account == null)
                return QueryResult<UserAccountReadDto>.Failure("user account not found");

            return QueryResult<UserAccountReadDto>.Successed(new UserAccountReadDto(
                userId: account.UserId,
                accountId: account.Id,
                userName: account.UserName,
                role: account.Role != null ?
                    new UserAccountRoleRead(account.Role.Id, account.Role.Name)
                    : null));
        }

        public async Task<QueryResult<UserAccountReadDto>> FindAccountByName(string userName)
        {
            UserAccount? account = await _dbContext.UserAccounts
                .FirstOrDefaultAsync(x => x.UserName == userName);

            if (account == null)
                return QueryResult<UserAccountReadDto>.Failure("user account not found");

            return QueryResult<UserAccountReadDto>.Successed(new UserAccountReadDto(
                userId: account.UserId,
                accountId: account.Id,
                userName: account.UserName,
                role: account.Role != null ?
                    new UserAccountRoleRead(account.Role.Id, account.Role.Name)
                    : null));
        }
    }
}
