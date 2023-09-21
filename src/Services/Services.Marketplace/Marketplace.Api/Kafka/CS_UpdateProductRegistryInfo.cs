using AutoMapper;
using Confluent.Kafka;
using Hangfire;
using Marketplace.Infrastructure.BusServices;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus.Serdes;
using Ozon.Bus;

namespace Marketplace.Api.Kafka
{
    public class CS_UpdateProductRegistryInfo
    {
        private readonly ILogger<CS_UpdateProductRegistryInfo> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        private readonly IMarketplaceProductBusService _productsBusService;

        public CS_UpdateProductRegistryInfo(
            IMarketplaceProductBusService productBusService,
            ILogger<CS_UpdateProductRegistryInfo> logger)
        {
            _productsBusService = productBusService;

            _logger = logger;

            _kafkaServer = "kafka-broker:9092";
        }

        [Queue("consumers")]
        public async Task ConsumeAsync()
        {
            _logger.LogCritical("[Start consuming]");

            CancellationTokenSource token = new CancellationTokenSource();

            try
            {
                Consumer<Ignore, SyncProductRegistryInfoAnswer> consumer = new Consumer<Ignore, SyncProductRegistryInfoAnswer>(
                    config: new ConsumerConfig()
                    {
                        GroupId = nameof(CS_UpdateProductRegistryInfo),
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        BootstrapServers = _kafkaServer,
                        EnableAutoCommit = false,
                        AutoCommitIntervalMs = 0,
                    },
                    valueDeserializator: new ServiceBusValueDeserializer<SyncProductRegistryInfoAnswer>(),
                    onExeption: (sender, exp) => {
                        Console.WriteLine("[CANCEL TOKEN]");
                        //consumer.close
                        token.Cancel();
                    });

                var observer = consumer.ConsumeObserv(new List<string> { "marketplace-products.syncProductRegistryInfo-answer" });

                // update marketplace product storages stock info
                observer.Subscribe((message) =>
                    {
                        _productsBusService.UpdateProductSeller(
                            marketplaceProductId: message.MarketplaceProductId,
                            externalSellerId: message.SellerId);

                        _productsBusService.UpdateProductInfo(
                            marketplaceProductId: message.MarketplaceProductId,
                            title: message.Title,
                            description: message.Description,
                            price: message.DefaultPrice);

                        _logger.LogCritical($"[{nameof(CS_UpdateProductRegistryInfo)}] msgs received: {message.Title}");
                    });

                consumer.Start(token.Token);
            }
            catch (Exception exp)
            {
                _logger.LogError("spec erorr: " + exp.Message);
            }
        }
    }
}
