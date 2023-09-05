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

        private readonly ILogger<AccountSecurityService> _logger;

        public AccountSecurityService(
            IServiceRepository<UserAccount> accountRepository,
             ILogger<AccountSecurityService> logger)
        {
            _logger = logger;
            _accountRepository = accountRepository;
        }

        public override async Task<CheckLogPassResponse> CheckLogpass(CheckLogPassRequest request, ServerCallContext context)
        {
            ISpecification<UserAccount> spec = new AccountByNameSpec(request.UserLogin);

            var user = _accountRepository.FirstOrDefault(spec);

            bool valid = user != null && user.PasswordHash.Equals(request.InputPassword);

            _logger.LogInformation("[check-logpass] + " + valid.ToString() + $" pass: {request.InputPassword}, login: {request.UserLogin}");

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
