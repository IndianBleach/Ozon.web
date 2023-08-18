using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DataQueries
{
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
