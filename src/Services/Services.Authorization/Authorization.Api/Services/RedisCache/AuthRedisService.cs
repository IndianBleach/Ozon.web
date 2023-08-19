using Authorization.Api.DTOs.Redis;
using Common.DataQueries;
using StackExchange.Redis;

namespace Authorization.Api.Services.RedisCache
{
    public class AuthRedisService : IAuthRedisService
    {
        private readonly IDatabase _redisDb;

        public AuthRedisService()
        {
            ConnectionMultiplexer _redis = ConnectionMultiplexer
                .Connect("ozon-auth-redis");

            _redisDb = _redis.GetDatabase();
        }

        private string DefaultExpiresTime()
        {
            DateTime dt = DateTime.Now;

            dt.AddDays(1);

            return dt.ToString();
        }

        private bool ValidExpireTime(string expireTime)
        {
            DateTime dt;

            if (DateTime.TryParse(expireTime, out dt) == true)
            {
                return dt <= DateTime.Now;
            }

            return false;
        }

        public async Task AddRedisUserAsync(AuthRedisUser authUser)
        {
            await _redisDb.HashSetAsync(
                key: authUser.UserId,
                new HashEntry[]
                {
                    new HashEntry("user_name", authUser.UserName),
                    new HashEntry("refresh_token", authUser.RefreshToken),
                    new HashEntry("expire_time", DefaultExpiresTime())
                });
        }


        public Task DeleteRedisUserAsync(string authUserId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckUserHasRefreshToken(
            string userId,
            string refreshToken)
        {
            HashEntry[] result = await _redisDb.HashGetAllAsync(
                userId);

            if (result.Length == 0)
                return false;

            HashEntry? tokenEntry = result
                .FirstOrDefault(x => x.Name == "refresh_token");

            HashEntry? timeEntry = result
                .FirstOrDefault(x => x.Name == "expire_time");

            if (tokenEntry.HasValue &&
                tokenEntry.Value.Value.Equals(refreshToken) &&
                timeEntry.HasValue &&
                ValidExpireTime(timeEntry.Value.Value))
                return true;

            return false;
        }
    }
}
