using AutoMapper;
using Confluent.Kafka;
using Hangfire;
using Marketplace.Infrastructure.BusServices;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus.Serdes;
using Ozon.Bus;
using Common.Repositories;
using Marketplace.Data.Entities.Sellers;
using Ozon.Bus.DTOs.ProductsRegistry;

namespace Marketplace.Api.Kafka
{
    public class CS_AddMarketplaceSeller
    {
        private readonly ILogger<CS_AddMarketplaceSeller> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        private readonly IServiceRepository<MarketplaceSeller> _sellerRepository;

        public CS_AddMarketplaceSeller(
            IServiceRepository<MarketplaceSeller> sellerRepository,
            ILogger<CS_AddMarketplaceSeller> logger)
        {
            _sellerRepository = sellerRepository;

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
                Consumer<string, ProductRegistryMarketplaceSeller> consumer = new Consumer<string, ProductRegistryMarketplaceSeller>(
                    config: new ConsumerConfig()
                    {
                        GroupId = nameof(CS_AddMarketplaceSeller),
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        BootstrapServers = _kafkaServer,
                        EnableAutoCommit = false,
                        AutoCommitIntervalMs = 0,
                    },
                    valueDeserializator: new ServiceBusValueDeserializer<ProductRegistryMarketplaceSeller>(),
                    onExeption: (sender, exp) => {
                        Console.WriteLine("[CANCEL TOKEN]");
                        //consumer.close
                        token.Cancel();
                    });

                var observer = consumer.ConsumeObserv(new List<string> { "products-marketplace.addMarketplaceSeller" });

                // update marketplace product storages stock info
                observer.Subscribe(
                    message =>
                    {
                        _logger.LogInformation($"[{nameof(CS_AddMarketplaceSeller)}] msgs received: {message.Name} {message.ExternalSellerId}");

                        if (!string.IsNullOrEmpty(message.ExternalSellerId))
                        {
                            _sellerRepository.Create(new MarketplaceSeller(
                                externalSellerId: message.ExternalSellerId,
                                title: message.Name,
                                verified: false,
                                isPopular: false,
                                created: DateTime.Now,
                                description: message.Description,
                                site: message.Site,
                                email: message.Email));
                        }
                        else
                        {
                            _logger.LogCritical($"msg delivery error, channel[products-marketplace.addMarketplaceSeller], null-exId");
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
