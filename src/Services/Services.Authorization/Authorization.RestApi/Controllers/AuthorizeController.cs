using Authorization.RestApi.DTOs.Jwt;
using Authorization.RestApi.DTOs.Users;
using Authorization.RestApi.Services.Jwt.Client;
using Authorization.RestApi.Services.Jwt.Tokens;
using Common.DataQueries;
using Microsoft.AspNetCore.Mvc;

namespace Authorization.RestApi.Controllers
{
    //[ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class AuthorizeController : ControllerBase
    {
        private readonly IJwtTokenWorker _jwtTokenHelper;

        private readonly IJwtClientService _jwtClientHelper;

        private readonly ILogger<AuthorizeController> _logger;

        public AuthorizeController(
            IJwtClientService jwtClientHelper,
            IJwtTokenWorker jwtTokenHelper,
            ILogger<AuthorizeController> logger)
        {
            _jwtClientHelper = jwtClientHelper;
            _jwtTokenHelper  = jwtTokenHelper;
            _logger = logger;
        }

        [HttpGet("/signout")]
        public async Task<IActionResult> SignOut(
            string account_id)
        {
            await _jwtClientHelper.SignOutAsync(account_id);

            return Ok();
        }

        [HttpGet("/refresh")]
        public async Task<IActionResult> RefreshJwt(
            string? account_id,
            string? refresh_token)
        {
            if (string.IsNullOrEmpty(account_id) ||
                string.IsNullOrEmpty(refresh_token))
                return BadRequest();

            QueryResult<JwtAuthorizeResponse> refreshQuery = await _jwtClientHelper.RefreshAccessTokenAsync(
                userAccountId: account_id,
                refreshToken: refresh_token);

            if (!refreshQuery.IsSuccessed)
                return BadRequest(refreshQuery);

            return Ok(refreshQuery.Value);
        }

        [HttpGet("/signin")]
        public async Task<IActionResult> SignIn(
            string authorize_token)
        {
            // get values from created account token
            UserAuthorizeData? user = _jwtTokenHelper.TryGetUserFromAccountToken(authorize_token);

            if (user == null)
                return BadRequest();

            // add to redis, write jwt
            QueryResult<JwtAuthorizeResponse> authQuery = await _jwtClientHelper.SignInAsync(
                userName: user.Login,
                userAccountId: user.AccountId);

            if (!authQuery.IsSuccessed)
                return BadRequest(authQuery);

            return Ok(authQuery.Value);
        }
    }
}