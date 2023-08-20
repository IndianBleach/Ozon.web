using Accounts.Data.Entities.AccountEntities;
using Accounts.Data.Entities.RoleEntities;
using Accounts.Infrastructure.Specifications;
using Accounts.Infrastructure.Specifications.Roles;
using Common.DataQueries;
using Common.Repositories;
using Common.Specifications;
using Data.Entities;
using Grpc.Accounts;
using Grpc.Accounts.Common;
using Grpc.Core;

namespace Accounts.Grpc.GrpcServices
{
    public class UserAccountGrpc : UserAccountGrpcService.UserAccountGrpcServiceBase
    {
        private readonly IServiceRepository<UserAccount> _accountRepository;

        private readonly IServiceRepository<UserRole> _roleRepository;

        private readonly IServiceRepository<ApplicationUser> _userRepository;

        public UserAccountGrpc(
            IServiceRepository<ApplicationUser> userRepository,
            IServiceRepository<UserRole> roleRepository,
            IServiceRepository<UserAccount> accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
        }

        public override async Task<CreateRoleRespone> CreateRole(CreateRoleRequest request, ServerCallContext context)
        {
            UserRole role = new UserRole(
                name: request.RoleName);

            QueryResult<string> query = _roleRepository.Create(role);

            if (!query.IsSuccessed)
                return new CreateRoleRespone
                {
                    QueryState = new QueryResultState
                    {
                        IsSuccessed = false,
                        ErrorMessage = query.StatusMessage
                    }
                };

            return new CreateRoleRespone
            {
                RoleId = query.Value
            };
        }

        public override async Task<CreateCreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            ApplicationUser user = new ApplicationUser(
                firstName: request.FirstName,
                lastName: request.SecondName,
                email: request.Email,
                dateCreated: DateTime.Now);

            QueryResult<string> query = _userRepository.Create(user);

            if (!query.IsSuccessed)
                return new CreateCreateUserResponse
                {
                    QueryState = new QueryResultState
                    {
                        ErrorMessage = query.StatusMessage,
                        IsSuccessed = false
                    }
                };

            return new CreateCreateUserResponse
            {
                UserId = query.Value
            };
        }

        public override async Task<CreateClientUserAccountResponse> CreateClientAccount(CreateClientUserAccountRequest request, ServerCallContext context)
        {
            ISpecification<UserRole> spec = new RoleByNameSpec("client");

            UserRole? getRole = _roleRepository.FirstOrDefault(spec);

            if (getRole == null)
                return new CreateClientUserAccountResponse
                {
                    QueryState = new QueryResultState
                    {
                        ErrorMessage = "Role (*client) not found",
                        IsSuccessed = false
                    }
                };

            UserAccount account = new UserAccount(
                userId: request.UserId,
                passwordHash: request.UserPassword,
                userName: request.UserName,
                getRole.Id);

            QueryResult<string> createQuery = _accountRepository.Create(account);

            if (!createQuery.IsSuccessed)
                return new CreateClientUserAccountResponse
                {
                    QueryState = new QueryResultState
                    {
                        ErrorMessage = createQuery.StatusMessage,
                        IsSuccessed = false
                    }
                };

            return new CreateClientUserAccountResponse
            {
                UserAccountId = createQuery.Value
            };
        }

        public override async Task<FindUserAccountByUserNameResponse> FindAccountByUserName(FindUserAccountByUserNameRequest request, ServerCallContext context)
        {
            ISpecification<UserAccount> spec = new AccountByNameSpec(request.UserName);

            UserAccount? result = _accountRepository.FirstOrDefault(spec);
                
            if (result == null)
                return new FindUserAccountByUserNameResponse()
                {
                    QueryState = new QueryResultState
                    {
                        ErrorMessage = "Account not found",
                        IsSuccessed = false
                    }
                };

            return new FindUserAccountByUserNameResponse
            {
                UserAccount = new UserAccountRead
                {
                    AccountId = result.Id,
                    Role = result.Role != null ?
                        new AccountRoleRead
                        {
                            RoleId = result.Role.Id,
                            RoleNormalizeName = result.Role.Name
                        }
                        : null,
                    UserId = result.UserId,
                    UserName = result.UserName
                }
            };
        }

        public override async Task<FindUserAccountByIdResponse> FindAccountById(FindUserAccountByIdRequest request, ServerCallContext context)
        {
            ISpecification<UserAccount> spec = new AccountByIdSpec(request.UserAccountId);

            var result = _accountRepository.FirstOrDefault(spec);

            if (result == null)
                return new FindUserAccountByIdResponse()
                {
                    QueryState = new QueryResultState
                    {
                        ErrorMessage = "Account not found",
                        IsSuccessed = false
                    }
                };

            return new FindUserAccountByIdResponse
            {
                UserAccount = new UserAccountRead
                {
                    AccountId = result.Id,
                    Role = result.Role != null ?
                        new AccountRoleRead
                        {
                            RoleId = result.Role.Id,
                            RoleNormalizeName = result.Role.Name
                        }
                        : null,
                    UserId = result.UserId,
                    UserName = result.UserName
                }
            };
        }
    }
}
