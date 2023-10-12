using AutoMapper;
using Common.Repositories;
using Confluent.Kafka;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;
using Ozon.Bus.Serdes;
using Hangfire;
using System.Threading;
using Marketplace.Infrastructure.BusServices;
using System.Collections.ObjectModel;
using System.Reactive;

namespace Storage.Api.Kafka.Services
{
    //storage-marketplace-updateProductStorageStockInfo

    public class CService_ProductStorageRegistration
    {
        private readonly ILogger<CService_ProductStorageRegistration> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        private readonly IMarketplaceProductBusService _productsBusService;

        private readonly IConsumerFactory _consumerFactory;

        public CService_ProductStorageRegistration(
            IConsumerFactory consumerFactory,
            IMarketplaceProductBusService productBusService,
            ILogger<CService_ProductStorageRegistration> logger)
        {
            _consumerFactory = consumerFactory;

            _productsBusService = productBusService;

            _logger = logger;

            _kafkaServer = "kafka-broker:9092";

            //var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
            //    new MarketplaceProductToStorageProductProfile()
            //}));
            //_mapper = new Mapper(config);

            //_productsUpdateStockStatusProducer = new Producer<string, List<StorageProductUpdateMarketplaceStockStatus>>(
            //    config: new ProducerConfig()
            //    {
            //        BootstrapServers = _kafkaServer,
            //        Acks = Acks.Leader,
            //        EnableBackgroundPoll = false,
            //    });
        }

        [Queue("consumers")]
        public async Task ConsumeAsync()
        {
            _logger.LogCritical("[Start consuming]");

            CancellationTokenSource token = new CancellationTokenSource();

            ConsumerWrapper<string, ReadOnlyCollection<StorageProductUpdateMarketplaceStockInfo>>? consumer = _consumerFactory.GetBatch<string, StorageProductUpdateMarketplaceStockInfo>();

            if (consumer != null)
            {
                try
                {
                    var data = consumer.ObservableData(new string[] { "storage-marketplace-updateProductStorageStockInfo" });

                    data.Subscribe((messages) =>
                    {
                        _logger.LogInformation($"[{nameof(CService_ProductStorageRegistration)}] msgs received: {messages.Count} {messages[0].MarketplaceProductId}");

                        _productsBusService.UpdateProductsStorageInfo(
                            products: messages);

                        // testing
                        consumer.Commit();
                    });

                    consumer.StartConsume(token.Token);
                }
                catch (Exception exp)
                { 
                    
                }
            }
        }
    }
}
