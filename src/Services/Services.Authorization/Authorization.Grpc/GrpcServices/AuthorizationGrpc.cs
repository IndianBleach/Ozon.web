using Authorization.Grpc;
using Authorization.Grpc.DTOs.Jwt;
using Authorization.Grpc.Services.Jwt.Client;
using Common.DataQueries;
using Grpc.Authorization;
using Grpc.Core;
using Authorization.Grpc.Extensions;

namespace Authorization.Grpc.Services
{
    public class AuthorizationGrpc : AuthorizationGrpcService.AuthorizationGrpcServiceBase
    {
        private readonly IJwtClientService _jwtClient;

        public AuthorizationGrpc(IJwtClientService jwtClient)
        {
            _jwtClient = jwtClient;
        }

        public override async Task<QueryState> JWTSignOut(JwtSignOutRequest request, ServerCallContext context)
        {
            await _jwtClient.SignOutAsync(request.UserAccountId);

            return GrpcExtensions.GoodQuery();
        }

        public override async Task<JwtSignInResponse> JWTSignIn(JwtSignInRequest request, ServerCallContext context)
        {
            QueryResult<JwtAuthorizeResponse> result = await _jwtClient.SignInAsync(
                userName: request.Name,
                userAccountId: request.UserAccountId);

            if (!result.IsSuccessed)
                return new JwtSignInResponse
                {
                    State = GrpcExtensions.BadQuery(result.StatusMessage)
                };

            return new JwtSignInResponse
            {
                Jwt = new JwtResponse
                {
                    AccessToken = result.Value.AccessToken,
                    RefreshToken = result.Value.RefreshToken
                }
            };
        }

        public override async Task<JwtRefreshTokenResponse> JWTRefreshToken(JwtRefreshTokenRequest request, ServerCallContext context)
        {
            QueryResult<JwtAuthorizeResponse> result = await _jwtClient.RefreshAccessTokenAsync(
                userAccountId: request.UserAccountId,
                refreshToken: request.RefreshToken);

            if (!result.IsSuccessed)
                return new JwtRefreshTokenResponse
                {
                    State = GrpcExtensions.BadQuery(result.StatusMessage)
                };

            return new JwtRefreshTokenResponse
            {
                Jwt = new JwtResponse
                {
                    AccessToken = result.Value.AccessToken,
                    RefreshToken = result.Value.RefreshToken
                }
            };
        }
    }
}