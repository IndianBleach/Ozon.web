using Common.DTOs.ApiRequests.Products;
using Confluent.Kafka;
using Ozon.Bus.DTOs.ProductsRegistry;
using Ozon.Bus;
using Ozon.Bus.DTOs.StorageService;
using Microsoft.Extensions.Logging;
using Grpc.Core;

namespace Storage.Api.Kafka.Producers
{
    internal class MarketplaceProducer : IMarketplaceProducer
    {
        private readonly Producer<string, AddStorageMessage> _marketplaceProducer;

        private readonly ILogger<MarketplaceProducer> _logger;

        public MarketplaceProducer(ILogger<MarketplaceProducer> logger)
        {
            _marketplaceProducer = new Producer<string, AddStorageMessage>(
                config: new ProducerConfig()
                {
                    BootstrapServers = "kafka-broker:9092",
                    Acks = Acks.Leader,
                    EnableBackgroundPoll = false,
                });

            _logger = logger;

        }

        public void AddMarketplaceStorage(int externalStorageId, string city, string street, string building)
        {
            _logger.LogInformation("+msg to [storage-marketplace.addMarketplaceStorage] storageId: " + externalStorageId);

            _marketplaceProducer.PublishMessage(
                toTopicAddr: "storage-marketplace.addMarketplaceStorage",
                message: new Message<string, AddStorageMessage>
                {
                    Key = Guid.NewGuid().ToString(),
                    Value = new AddStorageMessage
                    { 
                        ExternalStorageId = externalStorageId,
                        BuildingNumberAddr = building,
                        CityAddr = city,
                        StreetAddr = street
                    }
                },
                handler: (report) => {
                    _logger.LogWarning($"msg[storage-marketplace.addMarketplaceStorage] report: {report.Error.Reason} {report.Status.ToString()}");
                });
        }
    }
}
