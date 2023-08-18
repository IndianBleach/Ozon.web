using Authorization.Api.DTOs.Jwt;
using Authorization.Api.DTOs.Redis;
using Authorization.Api.Services.Jwt.Tokens;
using Authorization.Api.Services.RedisCache;
using Common.DataQueries;
using StackExchange.Redis;

namespace Authorization.Api.Services.Jwt.Client
{
    public class JwtClientService : IJwtClientService
    {
        private readonly IAuthRedisService _authRedisService;

        private readonly IJwtTokenWorker _jwtTokenService;

        public JwtClientService(
            IAuthRedisService authRedis,
            IJwtTokenWorker jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
            _authRedisService = authRedis;
        }

        // [REDIS]
        // validate access token
        // check user has token
        // write new access token
        // remove user-auth info

        public Task<QueryResult<JwtAuthorizeResponse>> RefreshAccessTokenAsync(string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<JwtAuthorizeResponse>> SignInAsync(string userName, string userPassword)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<JwtAuthorizeResponse>> SignOutAsync(string userAccountId)
        {
            throw new NotImplementedException();
        }

        public async Task<QueryResult<JwtAuthorizeResponse>> SignUpAsync(
            string userLogin,
            string password,
            string email)
        {
            // 1. check login
            // 2. create user

            // 3. add user-refresh token to redis +
            // 4. return +

            Console.WriteLine("TEST: login - " + userLogin);

            string userId = Guid.NewGuid().ToString();

            Console.WriteLine("FOR EXAMPLE AUTH ID: " + userId);

            string refreshToken = _jwtTokenService.WriteRefreshToken();

            Console.WriteLine("TEST: REFRESH TOKEN - " + refreshToken);

            string accessToken = _jwtTokenService.WriteAccessToken(
                userId: userId,
                login: userLogin);

            Console.WriteLine("TEST: ACCESS TOKEN - " + accessToken);

            await _authRedisService.AddRedisUserAsync(new AuthRedisUser(
                userId: userId,
                userName: userLogin,
                refreshToken: refreshToken));

            return QueryResult<JwtAuthorizeResponse>.Successed(
                new JwtAuthorizeResponse(
                    accessToken: accessToken,
                    refreshToken: refreshToken));
        }
    }
}
