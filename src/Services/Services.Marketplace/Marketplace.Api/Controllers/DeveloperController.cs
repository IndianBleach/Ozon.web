using Hangfire;
using Marketplace.Api.Kafka;
using Microsoft.AspNetCore.Mvc;
using Storage.Api.Kafka.Services;

namespace Marketplace.Api.Controllers
{
    public class DeveloperController : ControllerBase
    {
        private readonly IBackgroundJobClient _jobClient;

        public DeveloperController(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }

        [HttpPost("/consumers/")]
        public async Task<IActionResult> StartConsume()
        {
            //var id = _jobClient.Enqueue<CService_ProductStorageRegistration>((service) => service.ConsumeAsync());
            var id2 = _jobClient.Enqueue<CS_AddMarketplaceSeller>((service) => service.ConsumeAsync());
            var id3 = _jobClient.Enqueue<CS_AddMarketplaceStorage>((service) => service.ConsumeAsync());
            var id4 = _jobClient.Enqueue<CS_UpdateProductRegistryInfo>((service) => service.ConsumeAsync());
            var id5 = _jobClient.Enqueue<CService_ProductStorageRegistration>((service) => service.ConsumeAsync());

            return Ok(new List<string> { id2, id3, id4, id5 });
        }
    }
}
