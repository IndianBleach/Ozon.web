using Hangfire;
using Marketplace.Api.Kafka;
using Microsoft.AspNetCore.Mvc;

namespace Products.Api.Controllers
{
    public class DeveloperController : ControllerBase
    {
        private readonly IBackgroundJobClient _jobClient;

        public DeveloperController(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }

        [HttpPost("/consumers/productRegistry")]
        public async Task<IActionResult> Start_productRegistry()
        {
            var id = _jobClient.Enqueue<CS_SyncProductRegistryInfo>((service) => service.ConsumeAsync());

            return Ok(id);
        }
    }
}
