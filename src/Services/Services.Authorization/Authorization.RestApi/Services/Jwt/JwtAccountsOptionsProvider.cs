namespace Authorization.RestApi.Services.Jwt
{
    public class JwtAccountsOptionsProvider
    {
        public string ValidIssuer { get; private set; }

        public string ValidAudience { get; private set;}

        public string ValidSecretKey { get; private set; }

        public JwtAccountsOptionsProvider(
            string validIssuer,
            string validAudince,
            string validSecretKey)
        {
            ValidAudience = validAudince;
            ValidIssuer = validIssuer;
            ValidSecretKey = validSecretKey;

        }
    }
}
