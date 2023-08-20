using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Authorization.Grpc.Services.Jwt.Tokens
{
    public class JwtTokenWorker : IJwtTokenWorker
    {
        private readonly JwtOptionsProvider _jwtOptions;

        public JwtTokenWorker(JwtOptionsProvider jwtOptions)
        {
            _jwtOptions = jwtOptions;
        }

        public bool ValidateAccessToken(
            string accessToken)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            TokenValidationParameters validParams = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidAudience = _jwtOptions.Audince,
                ValidIssuer = _jwtOptions.Issuer
            };

            SecurityToken validatedToken;
            IPrincipal principal = handler.ValidateToken(
                accessToken,
                validParams,
                out validatedToken);

            return principal == null ? false : true;
        }

        public string WriteRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public string WriteAccessToken(
            string userId,
            string userName)
        {
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
            SigningCredentials signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenOptions = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audince,
                claims: new List<Claim>()
                { 
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userName),
                },
                expires: DateTime.Now.AddMinutes(_jwtOptions.LifeTimeMinutes),
                signingCredentials: signinCredentials
            );

            string tokenString = new JwtSecurityTokenHandler()
                .WriteToken(tokenOptions);

            return tokenString;
        }
    }
}
