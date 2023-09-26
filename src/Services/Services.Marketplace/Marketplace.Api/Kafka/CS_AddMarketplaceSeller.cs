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
using Ozon.Bus.Keys;
using System.Reactive;

namespace Marketplace.Api.Kafka
{
    public class CS_AddMarketplaceSeller
    {
        private readonly ILogger<CS_AddMarketplaceSeller> _logger;

        private readonly string _kafkaServer;

        private readonly IMapper _mapper;

        private readonly IServiceRepository<MarketplaceSeller> _sellerRepository;

        private readonly IConsumerFactory _consumerFactory;

        public CS_AddMarketplaceSeller(
            IConsumerFactory consumerFactory,
            IServiceRepository<MarketplaceSeller> sellerRepository,
            ILogger<CS_AddMarketplaceSeller> logger)
        {
            _consumerFactory = consumerFactory;

            _sellerRepository = sellerRepository;

            _logger = logger;

            _kafkaServer = "kafka-broker:9092";
        }

        [Queue("consumers")]
        public async Task ConsumeAsync()
        {
            _logger.LogInformation("[Start consuming...] CS_AddMarketplaceSeller");

            CancellationTokenSource token = new CancellationTokenSource();

            ConsumerWrapper<string, ProductRegistryMarketplaceSeller>? consumer = _consumerFactory.GetSingle<string, ProductRegistryMarketplaceSeller>();

            if (consumer != null)
            {
                try
                {
                    var data = consumer.ObservableData(new string[] { "products-marketplace.addMarketplaceSeller" });

                    data.Subscribe(
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
