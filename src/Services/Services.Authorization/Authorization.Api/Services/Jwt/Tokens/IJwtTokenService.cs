namespace Authorization.Api.Services.Jwt.Tokens
{
    // [REDIS]
    // validate access token
    // check user has token
    // write new access token
    // remove user-auth info

    public interface IJwtTokenService
    {
        bool ValidateAccessToken(string accessToken);

        bool CheckUserHasRefreshToken(string refreshToken);

        string WriteAccessToken(string userId);

        void RemoveUserAuthorizeInfo();
    }
}
