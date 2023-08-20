using Authorization.Grpc.DTOs.Jwt;
using Common.DataQueries;

namespace Authorization.Grpc.Services.Jwt.Client
{
    public interface IJwtClientService
    {
        Task<QueryResult<JwtAuthorizeResponse>> SignInAsync(
            string userName,
            string userAccountId);

        Task SignOutAsync(
            string userAccountId);

        Task<QueryResult<JwtAuthorizeResponse>> RefreshAccessTokenAsync(
            string userAccountId,
            string refreshToken);
    }
}
