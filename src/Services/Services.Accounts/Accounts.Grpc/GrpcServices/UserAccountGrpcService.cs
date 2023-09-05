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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Accounts.Grpc.GrpcServices
{
    public class UserAccountGrpc : UserAccountGrpcService.UserAccountGrpcServiceBase
    {
        private readonly IServiceRepository<UserAccount> _accountRepository;

        private readonly IServiceRepository<UserRole> _roleRepository;

        private readonly IServiceRepository<ApplicationUser> _userRepository;

        private readonly ILogger<UserAccountGrpc> _logger;

        public UserAccountGrpc(
            IServiceRepository<ApplicationUser> userRepository,
            IServiceRepository<UserRole> roleRepository,
            IServiceRepository<UserAccount> accountRepository,
            ILogger<UserAccountGrpc> logger)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public override async Task<CreateRoleRespone> CreateRole(CreateRoleRequest request, ServerCallContext context)
        {
            UserRole role = new UserRole(
                name: request.RoleName);

            QueryResult<UserRole> query = _roleRepository.Create(role);

            if (!query.IsSuccessed)
            {
                _logger.LogError($"[create role] {request.RoleName}");
                return new CreateRoleRespone
                {
                    QueryState = new QueryResultState
                    {
                        IsSuccessed = false,
                        ErrorMessage = query.StatusMessage
                    }
                };
            }

            return new CreateRoleRespone
            {
                RoleId = query.Value.Id
            };
        }

        public override async Task<CreateCreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            ApplicationUser user = new ApplicationUser(
                firstName: request.FirstName,
                lastName: request.SecondName,
                email: request.Email,
                dateCreated: DateTime.Now);

            QueryResult<ApplicationUser> query = _userRepository.Create(user);

            if (!query.IsSuccessed)
            {
                _logger.LogError($"[create user] {request.FirstName}");
                return new CreateCreateUserResponse
                {
                    QueryState = new QueryResultState
                    {
                        ErrorMessage = query.StatusMessage,
                        IsSuccessed = false
                    }
                };
            }

            _logger.LogError($"[create user (success)] {query.Value.Id}");
            return new CreateCreateUserResponse
            {
                UserId = query.Value.Id
            };
        }

        public override async Task<CreateClientUserAccountResponse> CreateClientAccount(CreateClientUserAccountRequest request, ServerCallContext context)
        {
            ISpecification<UserRole> spec = new RoleByNameSpec("client");

            UserRole? getRole = _roleRepository.FirstOrDefault(spec);

            _logger.LogError($"[ client role]= {getRole?.Name}");

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

            QueryResult<UserAccount> createQuery = _accountRepository.Create(account);

            _logger.LogError($"[create user ({createQuery.IsSuccessed})] accid= {createQuery.Value?.Id}");

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
                UserAccountId = createQuery.Value.Id
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
