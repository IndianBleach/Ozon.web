using Common.DTOs.Accounts;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO.Pipes;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Infrastructure.HelpServices
{
    public class JwtHelpService
    {
        private readonly string _key;

        private readonly string _issuer;

        private readonly string _audince;

        public JwtHelpService(
            string key,
            string issuer,
            string audince)
        {
            _audince = audince;
            _key = key;
            _issuer = issuer;
        }

        public UserClaimsJwtToken WriteUserClaimsToken(List<Claim> claims)
        {
            SymmetricSecurityKey secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_key));
            SigningCredentials signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken tokenOptions = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audince,
                claims: claims,
                signingCredentials: signinCredentials
            );

            return new UserClaimsJwtToken
            {
                ClaimsToken = new JwtSecurityTokenHandler()
                    .WriteToken(tokenOptions)
            };
        }
    }
}
