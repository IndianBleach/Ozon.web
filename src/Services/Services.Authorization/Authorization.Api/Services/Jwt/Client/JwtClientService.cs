using Authorization.Api.DTOs.Jwt;
using Common.DataQueries;

namespace Authorization.Api.Services.Jwt.Client
{
    public class JwtAuthorizeService : IJwtAuthorizeService
    {
        // [REDIS]
        // validate access token
        // check user has token
        // write new access token
        // remove user-auth info

        public Task<QueryResult<JwtAuthorizeResponse>> RefreshAccessTokenAsync(string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<JwtAuthorizeResponse>> SignInAsync(string userName, string userPassword)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<JwtAuthorizeResponse>> SignOutAsync(string userAccountId)
        {
            throw new NotImplementedException();
        }

        public Task<QueryResult<JwtAuthorizeResponse>> SignUpAsync()
        {
            throw new NotImplementedException();
        }
    }
}
