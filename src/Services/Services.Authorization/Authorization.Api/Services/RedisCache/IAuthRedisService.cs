using Authorization.Api.DTOs.Redis;
using Common.DataQueries;

namespace Authorization.Api.Services.RedisCache
{
    public interface IAuthRedisService
    {
        Task AddRedisUserAsync(
            AuthRedisUser authUser);

        Task DeleteRedisUserAsync(
            string authUserId);

        Task<bool> CheckUserHasRefreshToken(
            string userId,
            string refreshToken);
    }
}
