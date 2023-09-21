using GlobalGrpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataQueries
{
    public static class QueryResultExtensions
    {
        public static QueryStringIdResult ToGrpcStringIdResult(this QueryResult<string> queryValueId)
        {
            if (!queryValueId.IsSuccessed || queryValueId.Value == null)
            {
                return new QueryStringIdResult
                {
                    FailureValue = new QueryErrorResult
                    {
                        ErrorMessage = queryValueId.StatusMessage,
                        IsSuccessed = false
                    }
                };
            }

            return new QueryStringIdResult
            {
                SuccessValueId = queryValueId.Value
            };
        }

        public static QueryIntIdResult ToGrpcIntIdResult(this QueryResult<int> queryValueIntId)
        {
            if (!queryValueIntId.IsSuccessed || queryValueIntId.Value <= 0)
            {
                return new QueryIntIdResult
                {
                    FailureValue = new QueryErrorResult
                    {
                        ErrorMessage = queryValueIntId.StatusMessage,
                        IsSuccessed = false
                    }
                };
            }

            return new QueryIntIdResult
            {
                SuccessValueId = queryValueIntId.Value
            };
        }
    }

    public class QueryResultResponseRead<T>
    {
        public T? Value { get;  set; }

        public string? StatusMessage { get;  set; }

        public bool IsSuccessed { get;  set; }
    }

    public class QueryResult<T>
    {
        public T Value { get; private set; }

        public string? StatusMessage { get; private set; }

        public bool IsSuccessed { get; private set; }

        public static QueryResult<T> Successed(T value)
            => new QueryResult<T>
            {
                StatusMessage = null,
                IsSuccessed = true,
                Value = value
            };

        public static QueryResult<T> Failure(string errorMessage)
            => new QueryResult<T>
            {
                StatusMessage = errorMessage,
                IsSuccessed = false,
                Value = default(T)
            };
    }
}
