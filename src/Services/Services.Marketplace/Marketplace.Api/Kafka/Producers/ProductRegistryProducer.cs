using Confluent.Kafka;
using Ozon.Bus.DTOs.ProductsRegistry;
using Ozon.Bus;
using Ozon.Bus.DTOs.StorageService;
using System.Text;

namespace Marketplace.Api.Kafka.Producers
{
    public class ProductRegistryProducer : IProductRegistryProducer
    {
        private Producer<string, SyncProductRegistryInfoRequest> _productRegistryProducer;

        private readonly ILogger<ProductRegistryProducer> _logger;

        public ProductRegistryProducer(ILogger<ProductRegistryProducer> logger)
        {
            _productRegistryProducer = new Producer<string, SyncProductRegistryInfoRequest>(
                config: new ProducerConfig()
                {
                    BootstrapServers = "kafka-broker:9092",
                    Acks = Acks.Leader,
                    EnableBackgroundPoll = false,
                });

            _logger = logger;
        }

        public void UpdateProductRegistryInfo(string productId, string marketplaceProductId)
        {
            string responseChannel = "marketplace-products.syncProductRegistryInfo-answer";

            _logger.LogInformation($"[syncProductRegistryInfo-req] +msg " + marketplaceProductId + " " + productId);

            _productRegistryProducer.PublishMessage(
                toTopicAddr: "marketplace-products.syncProductRegistryInfo-req",
                message: new Message<string, SyncProductRegistryInfoRequest>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = new SyncProductRegistryInfoRequest
                    {
                        ExternalProductId = productId,
                        MarketplaceProductId = marketplaceProductId,
                        BusAsnwerChannel = responseChannel
                    }
                }, (report) => {
                    _logger.LogInformation($"[syncProductRegistryInfo-req] delivery {report.Status}");
                });
        }
    }
}
