namespace Authorization.RestApi.DTOs.Jwt
{
    public class JwtAuthorizeResponse
    {
        public string AccessToken { get; }

        public string RefreshToken { get; }

        public JwtAuthorizeResponse(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
