﻿using Authorization.Grpc.DTOs.Redis;
using Common.DataQueries;
using StackExchange.Redis;

namespace Authorization.Grpc.Services.RedisCache
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


        public async Task DeleteRedisUserAsync(string authUserId)
        {
            await _redisDb.HashDeleteAsync(
                key: authUserId,
                new RedisValue[]
                {
                    new RedisValue("user_name"),
                    new RedisValue("expire_time"),
                    new RedisValue("refresh_token"),
                });
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

        public async Task<QueryResult<HashEntry[]>> UpdateTokenExpireTime(
            string userId)
        {
            HashEntry[] result = await _redisDb.HashGetAllAsync(
                userId);

            if (result.Length == 0)
                return QueryResult<HashEntry[]>.Failure("auth-user not found");

            HashEntry? tokenEntry = result
                .FirstOrDefault(x => x.Name == "refresh_token");

            HashEntry? timeEntry = result
                .FirstOrDefault(x => x.Name == "expire_time");

            if (!tokenEntry.HasValue ||
                !timeEntry.HasValue)
                return QueryResult<HashEntry[]>.Failure("auth-user doesnt have some entries");

            await _redisDb.HashDeleteAsync(
                key: userId,
                hashField: "expire_time");

            await _redisDb.HashSetAsync(
                key: userId,
                hashField: "expire_time",
                value: DefaultExpiresTime());

            return QueryResult<HashEntry[]>.Successed(result);
        }
    }
}
