namespace Authorization.Api.Services.Jwt.Tokens
{
    public class JwtTokenService : IJwtTokenService
    {


        public bool CheckUserHasRefreshToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public void RemoveUserAuthorizeInfo()
        {
            throw new NotImplementedException();
        }

        public bool ValidateAccessToken(string accessToken)
        {
            throw new NotImplementedException();
        }

        public string WriteAccessToken(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
