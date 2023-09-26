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
using System.Collections.ObjectModel;
using System.Reactive;

namespace Storage.Api.Kafka.Services
{
    public class CService_ProductStorageRegistration
    {
        private readonly IServiceRepository<StorageProduct> _storageProductsRepository;

        private readonly ILogger<CService_ProductStorageRegistration> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        private readonly IConsumerFactory _consumerFactory;

        private readonly IProducerFactory _producerFactory;

        public CService_ProductStorageRegistration(
            IConsumerFactory consumerFactory,
            IProducerFactory producerFactory,
            IServiceRepository<StorageProduct> repo,
            ILogger<CService_ProductStorageRegistration> logger)
        {
            _producerFactory = producerFactory;
            _consumerFactory = consumerFactory;

            _storageProductsRepository = repo;

            _logger = logger;

            var config = new MapperConfiguration(cfg => cfg.AddProfiles(new List<Profile> {
                new MarketplaceProductToStorageProductProfile()
            }));
            _mapper = new Mapper(config);
        }

        [Queue("consumers")]
        public async Task ConsumeAsync()
        {
            _logger.LogCritical("[Start consuming]");

            CancellationTokenSource token = new CancellationTokenSource();

            ConsumerWrapper<string, ReadOnlyCollection<MarketplaceProductStorageRegistrationRead>>? consumer = _consumerFactory
                .GetBatch<string, MarketplaceProductStorageRegistrationRead>();

            if (consumer == null)
            {
                _logger.LogCritical("cannot find consumer [MarketplaceProductStorageRegistrationRead]");
                return;
            }

            ProducerWrapper<string, List<StorageProductUpdateMarketplaceStockInfo>>? producer = _producerFactory.GetBatch<string, StorageProductUpdateMarketplaceStockInfo>();

            try
            {
                var data = consumer.ObservableData(new string[] { "grpc-storage-registrationProducts" });

                data.Subscribe(messages =>
                    {
                        _logger.LogInformation($"[{nameof(CService_ProductStorageRegistration)}] msgs received: {messages.Count} {messages[0].MarketplaceProductId}");

                        _storageProductsRepository.AddRange(
                            _mapper.Map<IEnumerable<MarketplaceProductStorageRegistrationRead>, StorageProduct[]>(messages));

                        // +bus services.marketplace (update products status)[_batch]
                        _logger.LogInformation($"[{nameof(CService_ProductStorageRegistration)}] (+bus) send to marketplace (storage-marketplace-updateProductStorageStockInfo): {messages.Count} {messages[0].MarketplaceProductId}");

                        if (producer != null)
                        {
                            producer.PublishMessage(
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
                        }
                        else _logger.LogCritical("[StorageProductUpdateMarketplaceStockInfo] producer not found");
                        
                        // testing
                        consumer.Commit();
                    });

                consumer.StartConsume(token.Token);
            }
            catch (Exception exp)
            {
                _logger.LogCritical("[CService_ProductStorageRegistration] global error: " + exp.Message);
            }
        }
    }
}
