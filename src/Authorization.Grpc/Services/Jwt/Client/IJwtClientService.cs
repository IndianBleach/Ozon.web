using Authorization.Api.DTOs.Jwt;
using Common.DataQueries;

namespace Authorization.Api.Services.Jwt.Client
{
    public interface IJwtClientService
    {
        Task<QueryResult<JwtAuthorizeResponse>> SignInAsync(
            string userName,
            string userPassword);

        Task SignOutAsync(
            string userId);

        Task<QueryResult<JwtAuthorizeResponse>> RefreshAccessTokenAsync(
            string userId,
            string refreshToken);

        Task<QueryResult<JwtAuthorizeResponse>> SignUpAsync(
            string userLogin,
            string password,
            string email);
    }
}
