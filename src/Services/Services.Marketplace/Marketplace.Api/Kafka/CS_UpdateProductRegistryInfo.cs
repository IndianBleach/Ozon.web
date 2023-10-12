using AutoMapper;
using Confluent.Kafka;
using Hangfire;
using Marketplace.Infrastructure.BusServices;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus.Serdes;
using Ozon.Bus;
using System.Reactive;

namespace Marketplace.Api.Kafka
{
    public class CS_UpdateProductRegistryInfo
    {
        private readonly ILogger<CS_UpdateProductRegistryInfo> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        private readonly IConsumerFactory _consumerFactory;

        private readonly IMarketplaceProductBusService _productsBusService;

        public CS_UpdateProductRegistryInfo(
            IConsumerFactory consumerFactory,
            IMarketplaceProductBusService productBusService,
            ILogger<CS_UpdateProductRegistryInfo> logger)
        {
            _consumerFactory = consumerFactory;

            _productsBusService = productBusService;

            _logger = logger;

            _kafkaServer = "kafka-broker:9092";
        }

        [Queue("consumers")]
        public async Task ConsumeAsync()
        {
            _logger.LogCritical("[Start consuming]");

            CancellationTokenSource token = new CancellationTokenSource();

            ConsumerWrapper<string, SyncProductRegistryInfoAnswer>? consumer = _consumerFactory.GetSingle<string, SyncProductRegistryInfoAnswer>();

            if (consumer != null)
            {
                try
                {
                    var data = consumer.ObservableData(new List<string> { "marketplace-products.syncProductRegistryInfo-answer" });

                    data.Subscribe((message) =>
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

                        // testing
                        consumer.Commit();
                    });

                    consumer.StartConsume(token.Token);
                }
                catch (Exception exp)
                {
                    _logger.LogError("spec erorr: " + exp.Message);
                }
            }
        }
    }
}
