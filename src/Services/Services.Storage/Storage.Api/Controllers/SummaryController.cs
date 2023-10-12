using Microsoft.AspNetCore.Mvc;
using Storage.Api.ClickHouse;

namespace Storage.Api.Controllers
{
    public class SummaryController : ControllerBase
    {
        private IClickHouseStorageClient _clickHouseClient;

        private ILogger<SummaryController> _logger;

        public SummaryController(
            IClickHouseStorageClient clickHouseClient,
            ILogger<SummaryController> logger)
        {
            _logger = logger;
            _clickHouseClient = clickHouseClient;
        }

        // /storages/summary/marketplace/products/123
        [HttpGet("/[controller]/marketplace/products/{marketplaceProductId}")]
        public async Task<IActionResult> MarketplaceProductSummary(string marketplaceProductId)
        {
            _logger.LogInformation("get marketplace product storage summary: for prodId: " + marketplaceProductId);

            var result = await _clickHouseClient.GetMarketplaceProductStoragesSummary(
                marketplaceProductVariantId: marketplaceProductId);

            return Ok(result);
        }
    }
}
