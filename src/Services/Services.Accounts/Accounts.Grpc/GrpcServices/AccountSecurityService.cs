using Accounts.Data.Entities.AccountEntities;
using Accounts.Infrastructure.Specifications;
using Common.Repositories;
using Common.Specifications;
using Grpc.Accounts;
using Grpc.Accounts.Common;
using Grpc.Core;

namespace Accounts.Grpc.GrpcServices
{
    public class AccountSecurityService : AccountSecurityGrpcService.AccountSecurityGrpcServiceBase
    {
        private readonly IServiceRepository<UserAccount> _accountRepository;

        public AccountSecurityService(
            IServiceRepository<UserAccount> accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public override async Task<CheckLogPassResponse> CheckLogpass(CheckLogPassRequest request, ServerCallContext context)
        {
            ISpecification<UserAccount> spec = new AccountByNameSpec(request.UserLogin);

            var user = _accountRepository.FirstOrDefault(spec);

            bool valid = user != null && user.PasswordHash.Equals(request.InputPassword);

            if (!valid)
                return new CheckLogPassResponse
                {
                    QueryState = new QueryResultState
                    {
                        ErrorMessage = "invalid password or login",
                        IsSuccessed = false
                    }
                };

            return new CheckLogPassResponse
            {
                UserData = new SecurityAccountRead
                {
                    AccountId = user.Id,
                    UserId = user.UserId
                }
            };
        }
    }
}
