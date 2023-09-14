using Common.DataQueries;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Common.Logging
{
    public static class QueryResultExtensions
    {
        public static void LogFromQuery<TLogger, TQueryValue>(
            this QueryResultResponseRead<TQueryValue> query,
            ILogger<TLogger> logger,
            string identifyMsg)
        {
            if (query.IsSuccessed)
            {
                logger.LogInformation($"[{identifyMsg}]: SUCCESS, value: {query.Value}");
            }
            else
            {
                logger.LogWarning($"[{identifyMsg}]: FAILED, reason: {query.StatusMessage}");
            }

        }

        public static void LogFromQuery<TLogger, TQueryValue>(
            this QueryResult<TQueryValue> query,
            ILogger<TLogger> logger,
            string identifyMsg)
        {
            if (query.IsSuccessed)
            {
                logger.LogInformation($"[{identifyMsg}]: SUCCESS, value: {query.Value}");
            }
            else
            {
                logger.LogWarning($"[{identifyMsg}]: FAILED, reason: {query.StatusMessage}");
            }

        }
    }
}
