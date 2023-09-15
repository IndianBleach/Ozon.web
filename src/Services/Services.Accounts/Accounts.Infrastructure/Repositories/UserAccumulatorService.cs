using Accounts.Data.Entities.AccountEntities;
using Accounts.Data.Entities.RoleEntities;
using Accounts.Infrastructure.HelpServices;
using Accounts.Infrastructure.Specifications.Roles;
using Common.Constants;
using Common.DataQueries;
using Common.DTOs.Accounts;
using Common.Repositories;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.Repositories
{
    public class UserAccumulatorService : IUserAccumulatorService
    {
        private readonly IServiceRepository<ApplicationUser> _userRepository;

        private readonly IServiceRepository<UserAccount> _accountsRepository;

        private readonly IServiceRepository<UserRole> _roleRepository;

        private readonly JwtHelpService _jwtService;

        private readonly PasswordHashHelpService _passwordHashService;

        public UserAccumulatorService(
            PasswordHashHelpService passwordHashService,
            JwtHelpService jwtService,
            IServiceRepository<ApplicationUser> userRepository,
            IServiceRepository<UserAccount> accountsRepository,
            IServiceRepository<UserRole> roleRepository)
        {
            _roleRepository = roleRepository;
            _passwordHashService = passwordHashService;
            _jwtService = jwtService;
            _userRepository = userRepository;
            _accountsRepository = accountsRepository;
        }

        public QueryResult<UserClaimsJwtToken> RegisterUserIfNotExists(RegisterUserApiPost model)
        {
            if (_accountsRepository.Any(x => x.UserName.Equals(model.UserName)))
                throw new Exception("account's username already used");

            //var transaction = _userRepository.NewTransaction();

            try
            {
                var result = _userRepository.Create(new ApplicationUser(
                    firstName: model.FirstName,
                    lastName: model.LastName,
                    email: model.Email,
                    dateCreated: DateTime.Now));

                if (!result.IsSuccessed)
                    throw new Exception(result.StatusMessage);

                var role = _roleRepository.FirstOrDefault(new RoleByNameSpec(
                    roleName: "client"));

                if (role == null)
                    throw new Exception("role client doesn't exists");

                var createAccountResult = _accountsRepository.Create(new UserAccount(
                    userId: result.Value.Id,
                    passwordHash: _passwordHashService.HashPassword(model.Password),
                    userName: model.UserName,
                    roleId: role.Id));

                if (!createAccountResult.IsSuccessed)
                    throw new Exception(createAccountResult.StatusMessage);

                //transaction.Commit();

                List<Claim> claims = new List<Claim>
                {
                    new Claim(UserClaimsConstantNames.USER_ID, createAccountResult.Value.Id),
                    new Claim(UserClaimsConstantNames.USER_NAME, createAccountResult.Value.UserName),
                };

                return QueryResult<UserClaimsJwtToken>.Successed(
                    value: _jwtService.WriteUserClaimsToken(claims));
            }
            catch (Exception exp)
            {
                //transaction.Rollback();
                //transaction.Dispose();

                return QueryResult<UserClaimsJwtToken>.Failure(exp.Message);
            }
        }
    }
}
