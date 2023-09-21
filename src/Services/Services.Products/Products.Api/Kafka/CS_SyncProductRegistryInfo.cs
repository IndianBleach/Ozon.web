using AutoMapper;
using Confluent.Kafka;
using Hangfire;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus.Serdes;
using Ozon.Bus;
using Common.Repositories;
using Ozon.Bus.DTOs.ProductsRegistry;
using Products.Data.Entities;
using Products.Infrastructure.Specifications;

namespace Marketplace.Api.Kafka
{
    public class CS_SyncProductRegistryInfo
    {
        private readonly ILogger<CS_SyncProductRegistryInfo> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        private readonly IServiceRepository<Product> _productRepository;

        private Producer<string, SyncProductRegistryInfoAnswer> _syncProductRegistryProducer;

        public CS_SyncProductRegistryInfo(
            IServiceRepository<Product> productRepository,
            ILogger<CS_SyncProductRegistryInfo> logger)
        {
            _productRepository = productRepository;

            _syncProductRegistryProducer = new Producer<string, SyncProductRegistryInfoAnswer>(
                config: new ProducerConfig()
                {
                    BootstrapServers = "kafka-broker:9092",
                    Acks = Acks.Leader,
                    EnableBackgroundPoll = false,
                });

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
                Consumer<string, SyncProductRegistryInfoRequest> consumer = new Consumer<string, SyncProductRegistryInfoRequest>(
                    config: new ConsumerConfig()
                    {
                        GroupId = nameof(CS_SyncProductRegistryInfo),
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        BootstrapServers = _kafkaServer,
                        EnableAutoCommit = false,
                        AutoCommitIntervalMs = 0,
                    },
                    valueDeserializator: new ServiceBusValueDeserializer<SyncProductRegistryInfoRequest>(),
                    onExeption: (sender, exp) => {
                        Console.WriteLine("[CANCEL TOKEN]");
                        //consumer.close
                        token.Cancel();
                    });

                var observer = consumer.ConsumeObserv(new List<string> { "marketplace-products.syncProductRegistryInfo-req" });

                // update marketplace product storages stock info
                observer.Subscribe(
                    message =>
                    {
                        _logger.LogInformation($"[{nameof(CS_SyncProductRegistryInfo)}] msgs received:");

                        Product? findProduct = _productRepository.FirstOrDefault(new GetProductByIdSpec(message.ExternalProductId ?? string.Empty));

                        if (findProduct == null)
                            _logger.LogCritical("[marketplace-products.syncProductRegistryInfo-req] product not found " + message.ExternalProductId);
                        else
                        {
                            _syncProductRegistryProducer.PublishMessage(
                                toTopicAddr: message.BusAsnwerChannel,
                                message: new Message<string, SyncProductRegistryInfoAnswer>
                                {
                                    Key = Guid.NewGuid().ToString(),
                                    Value = new SyncProductRegistryInfoAnswer
                                    {
                                        DefaultPrice = findProduct.DefaultPrice,
                                        Description = findProduct.Description,
                                        SellerId = findProduct.SellerId,
                                        Title = findProduct.Title,
                                        MarketplaceProductId = message.MarketplaceProductId
                                    }
                                });
                        }
                       
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
