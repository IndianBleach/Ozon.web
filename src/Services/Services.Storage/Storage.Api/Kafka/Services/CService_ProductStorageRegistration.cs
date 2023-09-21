using AutoMapper;
using Common.Repositories;
using Confluent.Kafka;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;
using Storage.Api.Mappers.KafkaBus;
using Storage.Data.Entities.Products;
using Ozon.Bus.Serdes;
using Hangfire;
using System.Threading;

namespace Storage.Api.Kafka.Services
{
    public class CService_ProductStorageRegistration
    {
        private Producer<string, List<StorageProductUpdateMarketplaceStockInfo>> _productsUpdateStockStatusProducer;

        private readonly IServiceRepository<StorageProduct> _storageProductsRepository;

        private readonly ILogger<CService_ProductStorageRegistration> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        public CService_ProductStorageRegistration(
            IServiceRepository<StorageProduct> repo,
            ILogger<CService_ProductStorageRegistration> logger)
        {
            _storageProductsRepository = repo;

            _logger = logger;

            _kafkaServer = "kafka-broker:9092";

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
                new MarketplaceProductToStorageProductProfile()
            }));
            _mapper = new Mapper(config);

            _productsUpdateStockStatusProducer = new Producer<string, List<StorageProductUpdateMarketplaceStockInfo>>(
                config: new ProducerConfig()
                {
                    BootstrapServers = _kafkaServer,
                    Acks = Acks.Leader,
                    EnableBackgroundPoll = false,
                });
        }

        [Queue("consumers")]
        public async Task ConsumeAsync()
        {
            _logger.LogCritical("[Start consuming]");

            CancellationTokenSource token = new CancellationTokenSource();

            try
            {
                Consumer<string, List<MarketplaceProductStorageRegistrationRead>> consumer = new Consumer<string, List<MarketplaceProductStorageRegistrationRead>>(
                    config: new ConsumerConfig()
                    {
                        GroupId = nameof(CService_ProductStorageRegistration),
                        AutoOffsetReset = AutoOffsetReset.Latest,
                        BootstrapServers = _kafkaServer,
                        EnableAutoCommit = false,
                        AutoCommitIntervalMs = 0,
                    },
                    valueDeserializator: new ServiceBusValueDeserializer<List<MarketplaceProductStorageRegistrationRead>>(),
                    onExeption: (sender, exp) => {
                        Console.WriteLine("[CANCEL TOKEN]");
                        //consumer.close
                        token.Cancel();
                    });

                var observer = consumer.ConsumeObserv(new List<string> { "grpc-storage-registrationProducts" });

                observer.Subscribe(
                    messages =>
                    {
                        _logger.LogCritical($"[{nameof(CService_ProductStorageRegistration)}] msgs received: {messages.Count} {messages[0].MarketplaceProductId}");

                        _storageProductsRepository.AddRange(
                            _mapper.Map<IEnumerable<MarketplaceProductStorageRegistrationRead>, StorageProduct[]>(messages));

                        // +bus services.marketplace (update products status)[_batch]
                        _logger.LogWarning($"[{nameof(CService_ProductStorageRegistration)}] (+bus) send to marketplace (storage-marketplace-updateProductStorageStockInfo): {messages.Count} {messages[0].MarketplaceProductId}");

                        _productsUpdateStockStatusProducer.PublishMessage(
                            toTopicAddr: "storage-marketplace-updateProductStorageStockInfo",
                            message: new Message<string, List<StorageProductUpdateMarketplaceStockInfo>>
                            {
                                Key = Guid.NewGuid().ToString(),
                                Value = messages.Select(x => new StorageProductUpdateMarketplaceStockInfo
                                {
                                    MarketplaceProductId = x.MarketplaceProductId,
                                    NewStockStatusName = StorageServiceBusConstants.PRODUCT_STOCK_AVAILABLE_STATUS,
                                    StorageId = x.StorageId
                                }).ToList()
                            });

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
