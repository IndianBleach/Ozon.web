using Authorization.RestApi.DTOs.Users;
using System.Security.Claims;

namespace Authorization.RestApi.Services.Jwt.Tokens
{
    // [REDIS]
    // validate access token
    // check user has token
    // write new access token
    // remove user-auth info

    public interface IJwtTokenWorker
    {
        // get claims from accounts.hash

        UserAuthorizeData? TryGetUserFromAccountToken(string jwtToken);

        bool ValidateAccessToken(string accessToken);

        string WriteAccessToken(
            string userId,
            string login);

        string WriteRefreshToken();
    }
}
