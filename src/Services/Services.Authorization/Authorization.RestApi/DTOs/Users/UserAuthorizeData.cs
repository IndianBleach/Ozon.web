namespace Authorization.RestApi.DTOs.Users
{
    public class UserAuthorizeData
    {
        public string Login { get; private set; }

        public string AccountId { get; private set; }

        public UserAuthorizeData(
            string login,
            string id)
        {
            Login = login;
            AccountId = id;
        }
    }
}
