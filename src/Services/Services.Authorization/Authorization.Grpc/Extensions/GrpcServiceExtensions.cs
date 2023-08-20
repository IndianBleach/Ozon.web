using Grpc.Authorization;

namespace Authorization.Grpc.Extensions
{
    public static class GrpcExtensions
    {
        public static QueryState BadQuery(string statusMessage)
        {
            return new QueryState()
            {
                ErrorMessage = statusMessage,
                Successed = false
            };
        }

        public static QueryState GoodQuery()
        {
            return new QueryState()
            {
                ErrorMessage = null,
                Successed = true
            };
        }
    }
}
