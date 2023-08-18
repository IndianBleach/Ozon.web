namespace Authorization.Api.Services.Jwt
{
    public class JwtOptionsProvider
    {
        public string Issuer { get;}

        public string Audince { get; }

        public string Key { get; }

        public int LifeTimeMinutes { get; }

        public JwtOptionsProvider(
            string issuer, 
            string audince,
            string key,
            int lifeTimeInMinutes)
        {
            Issuer = issuer;
            Audince = audince;
            Key = key;
            LifeTimeMinutes = lifeTimeInMinutes;
        }
    }
}
