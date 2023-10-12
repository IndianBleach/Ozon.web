using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Simulator
{
    internal class ApiClient
    {
        private HttpClient _httpClient = new HttpClient();

        public ApiClient()
        {
            
        }

        internal async Task<TResponse> GetDataAsync<TResponse>(string url)
        {
            var stringResponse = await _httpClient.GetAsync(url)
                    .Result.Content
                    .ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<TResponse>(stringResponse);

            if (result == null)
                Console.WriteLine("[Des error] " + typeof(TResponse) + " " + url);

            return result;
        }
    }
}
