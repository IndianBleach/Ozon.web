
using Common.DataQueries;
using GlobalGrpc;
using Storage.Data.Entities;

namespace Storage.Grpc.GrpcUtils
{
    public static class GrpcProductStorageServiceExtensions
    {
        public static QueryIntIdResult FromQueryResult<T>(this QueryResult<T> query)
            where T : Storage.Data.Entities.TEntity
        {
            if (!query.IsSuccessed)
                return new QueryIntIdResult
                {
                    FailureValue = new QueryErrorResult
                    {
                        ErrorMessage = query.StatusMessage,
                        IsSuccessed = false
                    }
                };

            return new QueryIntIdResult
            {
                SuccessValueId = query.Value.Id,
            };
        }
    }
}
