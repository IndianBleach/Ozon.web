using Authorization.Api.DTOs.Redis;
using Common.DataQueries;
using StackExchange.Redis;

namespace Authorization.Api.Services.RedisCache
{
    public interface IAuthRedisService
    {
        Task<QueryResult<HashEntry[]>> UpdateTokenExpireTime(
            string userId);

        Task AddRedisUserAsync(
            AuthRedisUser authUser);

        Task DeleteRedisUserAsync(
            string authUserId);

        Task<bool> CheckUserHasRefreshToken(
            string userId,
            string refreshToken);
    }
}
