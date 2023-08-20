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

        public async Task<QueryResult<JwtAuthorizeResponse>> RefreshAccessTokenAsync(
            string userId,
            string refreshToken)
        {
            bool hasToken = await _authRedisService.CheckUserHasRefreshToken(
                userId: userId,
                refreshToken: refreshToken);

            if (!hasToken)
                return QueryResult<JwtAuthorizeResponse>.Failure(
                    errorMessage: "user refresh token not found or expired");

            QueryResult<HashEntry[]> updateResult = await _authRedisService.UpdateTokenExpireTime(
                userId: userId);

            if (!updateResult.IsSuccessed)
                return QueryResult<JwtAuthorizeResponse>.Failure(
                    errorMessage: "user update refresh token with error");

            HashEntry userLogin = updateResult.Value
                    .First(x => x.Name == "user_name");

            string newAccessToken = _jwtTokenService.WriteAccessToken(
                userId,
                userLogin.Value);

            return QueryResult<JwtAuthorizeResponse>.Successed(new JwtAuthorizeResponse(
                accessToken: newAccessToken,
                refreshToken: refreshToken));
        }

        public Task<QueryResult<JwtAuthorizeResponse>> SignInAsync(
            string userName, 
            string userPassword)
        {
            // 1 accounts.findByName
            // 2 accounts.validatePassword

            // add auth user to redis
            // return


            throw new NotImplementedException();
        }

        public async Task SignOutAsync(string userId)
        {
            await _authRedisService.DeleteRedisUserAsync(
                authUserId: userId);
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

            string userId = Guid.NewGuid().ToString();

            string refreshToken = _jwtTokenService.WriteRefreshToken();

            string accessToken = _jwtTokenService.WriteAccessToken(
                userId: userId,
                login: userLogin);

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
