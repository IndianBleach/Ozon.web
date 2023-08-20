using Accounts.Infrastructure.Specifications;
using Common.DataQueries;
using Common.Repositories;
using Common.Specifications;
using Grpc.Accounts;
using Grpc.Accounts.Common;
using Grpc.Core;

namespace Accounts.Grpc.GrpcServices
{
    public class UserAccountGrpc : UserAccountGrpcService.UserAccountGrpcServiceBase
    {
        private readonly IServiceRepository<UserAccount> _accountRepository;

        public UserAccountGrpc(IServiceRepository<UserAccount> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public override async Task<FindUserAccountByUserNameResponse> FindAccountByUserName(FindUserAccountByUserNameRequest request, ServerCallContext context)
        {
            ISpecification<UserAccount> spec = (ISpecification<UserAccount>)new AccountByNameSpec(request.UserName);

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
                UserAccount = new UserAccount
                {
                    AccountId = result.AccountId,
                    Role = result.Role != null ?
                        new AccountRole
                        {
                            RoleId = result.Role.RoleId,
                            RoleNormalizeName = result.Role.RoleNormalizeName
                        }
                        : null,
                    UserId = result.UserId,
                    UserName = result.UserName
                }
            };
        }

        public override async Task<FindUserAccountByIdResponse> FindAccountById(FindUserAccountByIdRequest request, ServerCallContext context)
        {
            ISpecification<UserAccount> spec = (ISpecification<UserAccount>)new AccountByIdSpec(request.UserAccountId);

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
                UserAccount = new UserAccount
                {
                    AccountId = result.AccountId,
                    Role = result.Role != null ?
                        new AccountRole
                        {
                            RoleId = result.Role.RoleId,
                            RoleNormalizeName = result.Role.RoleNormalizeName
                        }
                        : null,
                    UserId = result.UserId,
                    UserName = result.UserName
                }
            };
        }
    }
}
