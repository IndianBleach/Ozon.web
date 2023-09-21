using Authorization.RestApi.DTOs.Jwt;
using Authorization.RestApi.DTOs.Redis;
using Authorization.RestApi.Services.Jwt.Tokens;
using Authorization.RestApi.Services.RedisCache;
using Common.DataQueries;
using StackExchange.Redis;

namespace Authorization.RestApi.Services.Jwt.Client
{
    public class JwtClientService : IJwtClientService
    {
        private readonly IAuthRedisService _authRedisService;

        private readonly IJwtTokenWorker _jwtTokenService;

        private readonly ILogger<JwtClientService> _logger;

        public JwtClientService(
            IAuthRedisService authRedis,
            IJwtTokenWorker jwtTokenService,
            ILogger<JwtClientService> logger)
        {
            _jwtTokenService = jwtTokenService;
            _authRedisService = authRedis;
            _logger = logger;
        }

        public async Task<QueryResult<JwtAuthorizeResponse>> RefreshAccessTokenAsync(
            string userAccountId,
            string refreshToken)
        {
            _logger.LogWarning($"[RefreshAccessTokenAsync] {userAccountId} {refreshToken}");

            bool hasToken = await _authRedisService.CheckUserHasRefreshToken(
                userId: userAccountId,
                refreshToken: refreshToken);

            if (!hasToken)
                return QueryResult<JwtAuthorizeResponse>.Failure(
                    errorMessage: "user refresh token not found or expired");

            QueryResult<HashEntry[]> updateResult = await _authRedisService.UpdateTokenExpireTime(
                userId: userAccountId);

            if (!updateResult.IsSuccessed)
                return QueryResult<JwtAuthorizeResponse>.Failure(
                    errorMessage: "user update refresh token with error");

            HashEntry? userLogin = updateResult.Value
                    .FirstOrDefault(x => x.Name == "user_name");

            if (userLogin?.Value == null)
                return QueryResult<JwtAuthorizeResponse>.Failure(
                    errorMessage: "user update refresh token with error");

            string newAccessToken = _jwtTokenService.WriteAccessToken(
                userAccountId,
                userLogin.Value.Value);

            return QueryResult<JwtAuthorizeResponse>.Successed(new JwtAuthorizeResponse(
                accessToken: newAccessToken,
                refreshToken: refreshToken));
        }

        public async Task<QueryResult<JwtAuthorizeResponse>> SignInAsync(
            string userName, 
            string userAccountId)
        {
            try
            {
                string refreshToken = _jwtTokenService.WriteRefreshToken();

                string accessToken = _jwtTokenService.WriteAccessToken(
                    userId: userAccountId,
                    login: userName);

                await _authRedisService.AddRedisUserAsync(new AuthRedisUser(
                    userId: userAccountId,
                    userName: userName,
                    refreshToken: refreshToken));

                return QueryResult<JwtAuthorizeResponse>.Successed(
                    new JwtAuthorizeResponse(
                        accessToken: accessToken,
                        refreshToken: refreshToken));
            }
            catch (Exception exp)
            {
                return QueryResult<JwtAuthorizeResponse>.Failure(exp.Message);
            }
        }

        public async Task SignOutAsync(string userAccountId)
        {
            await _authRedisService.DeleteRedisUserAsync(
                authUserId: userAccountId);
        }
    }
}
