using Common.DataQueries;
using GlobalGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Grpc.Extensions
{
    public static class GrpcGlobalTools
    {
        

        public static QueryStringIdResult FromQueryResult(QueryResult<string> query)
        {
            if (!query.IsSuccessed || string.IsNullOrEmpty(query.Value))
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
                SuccessValueId = query.Value,
            };
        }

        public static QueryIntIdResult FromQueryResult(QueryResult<int> query)
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
                SuccessValueId = query.Value,
            };
        }


        public static QueryStringIdResult Failure(string? errorMessage)
        {
            return new QueryStringIdResult()
            {
                FailureValue = new QueryErrorResult()
                {
                    ErrorMessage = errorMessage,
                    IsSuccessed = false
                }
            };
        }

        public static QueryStringIdResult Success(string valueId)
        {
            return new QueryStringIdResult()
            {
                SuccessValueId = valueId,
            };
        }
    }
}
