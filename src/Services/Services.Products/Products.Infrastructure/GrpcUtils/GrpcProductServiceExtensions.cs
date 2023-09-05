
using Common.DataQueries;
using GlobalGrpc;
using Products.Data.Entities;

namespace Products.Grpc.GrpcUtils
{
    public static class GrpcProductServiceExtensions
    {
        public static QueryStringIdResult FromQueryResult<T>(this QueryResult<T> query)
            where T : Products.Data.Entities.TEntity
        {
            if (!query.IsSuccessed || string.IsNullOrEmpty(query.Value.Id))
                return new QueryStringIdResult
                {
                    FailureValue = new QueryErrorResult
                    {
                        ErrorMessage = query.StatusMessage,
                        IsSuccessed = false
                    }
                };

            return new QueryStringIdResult
            {
                SuccessValueId = query.Value.Id,
            };
        }
    }
}
