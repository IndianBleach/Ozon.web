using Authorization.Api.DTOs.Jwt;
using Common.DataQueries;

namespace Authorization.Api.Services.Jwt.Client
{
    public interface IJwtAuthorizeService
    {
        Task<QueryResult<JwtAuthorizeResponse>> SignInAsync(
            string userName,
            string userPassword);

        Task<QueryResult<JwtAuthorizeResponse>> SignOutAsync(
            string userAccountId);

        Task<QueryResult<JwtAuthorizeResponse>> RefreshAccessTokenAsync(
            string accessToken);

        Task<QueryResult<JwtAuthorizeResponse>> SignUpAsync();
    }
}
