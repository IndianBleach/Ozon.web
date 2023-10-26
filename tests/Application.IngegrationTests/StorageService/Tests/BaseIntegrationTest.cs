using Common.DataQueries;
using Hangfire.MemoryStorage.Database;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Storage.Data.Context;
using Storage.Data.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Application.IngegrationTests.StorageService.Tests
{
    public abstract class BaseIntegrationTest : IClassFixture<StorageServiceIntegrationTestApiFactory>
    {
        protected readonly StorageDbContext StorageServiceDbContext;

        protected readonly HttpClient StorageApiClient;

        protected async Task<ApiResponseRead<TResponse>> ApiPostAsync<TResponse, TMessage>(
            string uri,
            TMessage body)
        {
             var response = await StorageApiClient.PostAsJsonAsync(uri, body);
            
            var s = await response.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<ApiResponseRead<TResponse>>(
                await response.Content.ReadAsStringAsync());

            return obj;
        }


        protected BaseIntegrationTest(StorageServiceIntegrationTestApiFactory factory)
        {
            var scope = factory.Services.CreateScope();
            
            StorageServiceDbContext = scope.ServiceProvider.GetRequiredService<StorageDbContext>();

            StorageApiClient = factory.CreateClient();

            int t2 = -2;
        }
    }
}
