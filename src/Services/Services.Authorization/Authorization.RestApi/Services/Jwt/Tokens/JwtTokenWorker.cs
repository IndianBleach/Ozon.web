using Authorization.RestApi.DTOs.Users;
using Common.Constants;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Authorization.RestApi.Services.Jwt.Tokens
{
    public class JwtTokenWorker : IJwtTokenWorker
    {
        private readonly JwtOptionsProvider _jwtOptions;

        private readonly JwtAccountsOptionsProvider _jwtAccountsOptions;

        public JwtTokenWorker(
            JwtOptionsProvider jwtOptions, JwtAccountsOptionsProvider jwtAccountsOptions)
        {
            _jwtOptions = jwtOptions;
            _jwtAccountsOptions = jwtAccountsOptions;
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
                ValidateIssuerSigningKey = false,
                ValidAudience = _jwtOptions.Audince,
                ValidIssuer = _jwtOptions.Issuer
            };

            IPrincipal principal = handler.ValidateToken(
                accessToken,
                validParams,
                out _);

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
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Key));
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

        public UserAuthorizeData? TryGetUserFromAccountToken(string jwtToken)
        {
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtAccountsOptions.ValidSecretKey));

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            TokenValidationParameters validParams = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidAudience = _jwtAccountsOptions.ValidAudience,
                ValidIssuer = _jwtAccountsOptions.ValidIssuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = secretKey
            };

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(
                jwtToken,
                validParams,
                out _);

            var id = principal.FindFirst(x => x.Type == UserClaimsConstantNames.USER_ID);
            var login = principal.FindFirst(x => x.Type == UserClaimsConstantNames.USER_NAME);

            if (id == null || login == null)
                return null;

            return new UserAuthorizeData(
                login: login.Value,
                id: id.Value);
        }
    }
}
