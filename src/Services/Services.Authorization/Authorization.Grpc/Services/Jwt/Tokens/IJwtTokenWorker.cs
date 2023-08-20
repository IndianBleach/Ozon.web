namespace Authorization.Grpc.Services.Jwt.Tokens
{
    // [REDIS]
    // validate access token
    // check user has token
    // write new access token
    // remove user-auth info

    public interface IJwtTokenWorker
    {
        bool ValidateAccessToken(string accessToken);

        string WriteAccessToken(
            string userId,
            string login);

        string WriteRefreshToken();
    }
}
