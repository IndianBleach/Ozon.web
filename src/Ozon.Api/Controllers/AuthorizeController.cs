using Grpc.Authorization;
using Grpc.Accounts;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Ozon.Api.DTOs.Authorization;

namespace Ozon.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthorizeController : ControllerBase
    {
        private readonly ILogger<AuthorizeController> _logger;

        private GrpcChannel _authChannel;

        private GrpcChannel _accountChannel;

        public AuthorizeController(
            ILogger<AuthorizeController> logger,
            IConfiguration config)
        {
            _logger = logger;
        }

        [HttpPost("role")]
        [Consumes("application/json")]
        public async Task<IActionResult> Role(string roleName)
        {
            var accountClient = new UserAccountGrpcService.UserAccountGrpcServiceClient(_accountChannel);

            var res = accountClient.CreateRole(new CreateRoleRequest
            {
                RoleName = roleName
            });

            return Ok(res);
        }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUpUser(
            [FromBody]AuthorizeUserSignUpRead model)
        {
            Console.WriteLine("USERNAME (SIGNUP) " + model.UserName);

            var authClient = new AuthorizationGrpcService.AuthorizationGrpcServiceClient(_authChannel);

            var accountClient = new UserAccountGrpcService.UserAccountGrpcServiceClient(_accountChannel);

            var res = accountClient.CreateUser(new CreateUserRequest
            {
                FirstName = model.FirstName,
                Email = model.Email,
                SecondName = model.SecondName
            });

            _logger.LogInformation("[UserId] " + res.UserId);

            if (res.HasUserId)
            {
                var accountResult = accountClient.CreateClientAccount(new CreateClientUserAccountRequest
                {
                    UserId = res.UserId,
                    UserName = model.UserName,
                    UserPassword = model.Password
                });

                _logger.LogInformation("[AccountId] " + accountResult.UserAccountId);

                var resp = authClient.JWTSignIn(new JwtSignInRequest
                {
                    Name = model.UserName,
                    UserAccountId = accountResult.UserAccountId
                });

                return Ok(resp);
            }

            return Ok(res);
        }

        [HttpPost("signin")]
        [Consumes("application/json")]
        public async Task<IActionResult> SignInUser([FromBody]AuthorizeUserPut userModel)
        {
            var authClient = new AuthorizationGrpcService.AuthorizationGrpcServiceClient(_authChannel);

            var accountSecClient = new AccountSecurityGrpcService.AccountSecurityGrpcServiceClient(_accountChannel);

            var result = accountSecClient.CheckLogpass(new CheckLogPassRequest()
            {
                InputPassword = userModel.UserInputPassword,
                UserLogin = userModel.UserName
            });

            _logger.LogInformation("[check logpass] accid " + result.UserData?.AccountId);

            if (result.QueryState != null &&
                result.QueryState.IsSuccessed == false)
                return Ok(result.QueryState);

            var resp = authClient.JWTSignIn(new JwtSignInRequest
            {
                Name = userModel.UserName,
                UserAccountId = result.UserData.AccountId
            });

            return Ok(resp.Jwt);
        }

        [HttpPut("refresh")]
        [Consumes("application/json")]
        public async Task<IActionResult> RefreshJwtToken(
            string refr_token,
            string user_accid)
        {
            var authClient = new AuthorizationGrpcService.AuthorizationGrpcServiceClient(_authChannel);

            var resp = authClient.JWTRefreshTokenAsync(new JwtRefreshTokenRequest()
            {
                RefreshToken = refr_token,
                UserAccountId = user_accid
            });

            return Ok(resp);
        }
    }
}