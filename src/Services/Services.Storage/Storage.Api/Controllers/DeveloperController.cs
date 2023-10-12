using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Storage.Api.ClickHouse;
using Storage.Api.Kafka.Services;

namespace Storage.Api.Controllers
{
    public class DeveloperController : ControllerBase
    {
        private readonly IBackgroundJobClient _jobClient;

        private IClickHouseStorageClient _chCLient;

        public DeveloperController(
            IClickHouseStorageClient chClient,
            IBackgroundJobClient jobClient)
        {
            _chCLient = chClient;
            _jobClient = jobClient;
        }

        [HttpPost("/consumers/")]
        public async Task<IActionResult> StartConsumers()
        {
            var id = _jobClient.Enqueue<CService_ProductStorageRegistration>((service) => service.ConsumeAsync());

            return Ok(id);
        }
    }
}
