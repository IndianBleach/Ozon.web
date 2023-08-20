namespace Authorization.Grpc.DTOs.Redis
{
    public class AuthRedisUser
    {
        public string UserId { get; }

        public string UserName { get; }

        public string RefreshToken { get; }

        public AuthRedisUser(
            string userId,
            string userName,
            string refreshToken)
        {
            UserId = userId;
            UserName = userName;
            RefreshToken = refreshToken;
        }
    }
}
