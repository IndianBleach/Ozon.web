using Common.DTOs.ApiRequests.Products;
using Confluent.Kafka;
using Ozon.Bus;
using Ozon.Bus.DTOs.ProductsRegistry;
using Ozon.Bus.DTOs.StorageService;

namespace Products.Api.Kafka.Producers
{
    public class MarketplaceProducer : IMarketplaceProducer
    {
        private readonly Producer<string, ProductRegistryMarketplaceSeller> _marketplaceSellerProducer;

        private readonly ILogger<MarketplaceProducer> _logger;

        public MarketplaceProducer(ILogger<MarketplaceProducer> logger)
        {
            _marketplaceSellerProducer = new Producer<string, ProductRegistryMarketplaceSeller>(
                config: new ProducerConfig()
                {
                    BootstrapServers = "kafka-broker:9092",
                    Acks = Acks.Leader,
                    EnableBackgroundPoll = false,
                });

            _logger = logger;

        }

        public void AddMarketplaceSeller(
            ProductSellerApiPost seller,
            string sellerId)
        {
            _logger.LogInformation("+msg to [products-marketplace.addMarketplaceSeller] sellerId: " + sellerId);

            _marketplaceSellerProducer.PublishMessage(
                toTopicAddr: "products-marketplace.addMarketplaceSeller",
                message: new Message<string, ProductRegistryMarketplaceSeller>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = new ProductRegistryMarketplaceSeller
                    {
                        Description = seller.Description,
                        Email = seller.Email,
                        ExternalSellerId = sellerId,
                        Name = seller.Title,
                        Site = seller.Site
                    }
                },
                handler: (report) => {
                    _logger.LogWarning($"msg[products-marketplace.addMarketplaceSeller] report: {report.Error.Reason} {report.Status.ToString()}");
                });
        }
    }
}
