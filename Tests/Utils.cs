using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    internal class Utils
    {
        private ILogger<Utils> _logger;

        private Random _rnd = new Random();

        private readonly HttpClient _httpClient;

        public int Rand(int min, int max) => _rnd.Next(min, max);

        public Utils()
        {
            _httpClient = new HttpClient();

            var loggerFactory = LoggerFactory.Create(
                builder => builder
                            .AddConsole()
                            .SetMinimumLevel(LogLevel.Debug)
            );
            _logger = loggerFactory.CreateLogger<Utils>();
        }

        public async Task<TResponse> GetDataAsync<TResponse>(string url)
        {
            var stringResponse = await _httpClient.GetAsync(url)
                    .Result.Content
                    .ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TResponse>(stringResponse);

            if (result == null)
                _logger.LogCritical("[Des error] " + typeof(TResponse) + " " + url);

            return result;
        }
    }
}
