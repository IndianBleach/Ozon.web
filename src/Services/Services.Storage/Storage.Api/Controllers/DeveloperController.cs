using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Storage.Api.Kafka.Services;

namespace Storage.Api.Controllers
{
    public class DeveloperController : ControllerBase
    {
        private readonly IBackgroundJobClient _jobClient;

        public DeveloperController(IBackgroundJobClient jobClient)
        {
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
