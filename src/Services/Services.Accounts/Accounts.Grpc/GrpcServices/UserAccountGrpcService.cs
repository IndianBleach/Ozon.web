using Common.DataQueries;
using Grpc.Accounts;
using Grpc.Accounts.Common;
using Grpc.Core;
using Infrastructure.DataServices;

namespace Accounts.Grpc.GrpcServices
{
    public class UserAccountGrpc : UserAccountGrpcService.UserAccountGrpcServiceBase
    {
        private readonly IUserAccountService _userAccountService;

        public UserAccountGrpc(IUserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        public override async Task<FindUserAccountByUserNameResponse> FindAccountByUserName(FindUserAccountByUserNameRequest request, ServerCallContext context)
        {
            var result = await _userAccountService.FindAccountByName(
                userName: request.UserName);

            if (!result.IsSuccessed)
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
                    AccountId = result.Value.AccountId,
                    Role = result.Value.Role != null ?
                        new AccountRole
                        {
                            RoleId = result.Value.Role.RoleId,
                            RoleNormalizeName = result.Value.Role.RoleName
                        }
                        : null,
                    UserId = result.Value.UserId,
                    UserName = result.Value.UserName
                }
            };
        }

        public override async Task<FindUserAccountByIdResponse> FindAccountById(FindUserAccountByIdRequest request, ServerCallContext context)
        {
            var result = await _userAccountService.FindAccountById(
                userAccountId: request.UserAccountId);

            if (!result.IsSuccessed)
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
                    AccountId = result.Value.AccountId,
                    Role = result.Value.Role != null ?
                        new AccountRole
                        {
                            RoleId = result.Value.Role.RoleId,
                            RoleNormalizeName = result.Value.Role.RoleName
                        }
                        : null,
                    UserId = result.Value.UserId,
                    UserName = result.Value.UserName
                }
            };
        }
    }
}
