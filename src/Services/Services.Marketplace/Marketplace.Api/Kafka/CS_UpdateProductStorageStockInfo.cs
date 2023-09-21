using AutoMapper;
using Common.Repositories;
using Confluent.Kafka;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;
using Ozon.Bus.Serdes;
using Hangfire;
using System.Threading;
using Marketplace.Infrastructure.BusServices;

namespace Storage.Api.Kafka.Services
{
    //storage-marketplace-updateProductStorageStockInfo

    public class CService_ProductStorageRegistration
    {
        private readonly ILogger<CService_ProductStorageRegistration> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        private readonly IMarketplaceProductBusService _productsBusService;

        public CService_ProductStorageRegistration(
            IMarketplaceProductBusService productBusService,
            ILogger<CService_ProductStorageRegistration> logger)
        {
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

            try
            {
                Consumer<string, List<StorageProductUpdateMarketplaceStockInfo>> consumer = new Consumer<string, List<StorageProductUpdateMarketplaceStockInfo>>(
                    config: new ConsumerConfig()
                    {
                        GroupId = nameof(CService_ProductStorageRegistration),
                        AutoOffsetReset = AutoOffsetReset.Latest,
                        BootstrapServers = _kafkaServer,
                        EnableAutoCommit = false,
                        AutoCommitIntervalMs = 0,
                    },
                    valueDeserializator: new ServiceBusValueDeserializer<List<StorageProductUpdateMarketplaceStockInfo>>(),
                    onExeption: (sender, exp) => {
                        Console.WriteLine("[CANCEL TOKEN]");
                        //consumer.close
                        token.Cancel();
                    });

                var observer = consumer.ConsumeObserv(new List<string> { "storage-marketplace-updateProductStorageStockInfo" });

                // update marketplace product storages stock info
                observer.Subscribe((messages) =>
                    {
                        _logger.LogCritical($"[{nameof(CService_ProductStorageRegistration)}] msgs received: {messages.Count} {messages[0].MarketplaceProductId}");

                        _productsBusService.UpdateProductsStorageInfo(
                            products: messages);

                        // testing
                        consumer.Commit();
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
