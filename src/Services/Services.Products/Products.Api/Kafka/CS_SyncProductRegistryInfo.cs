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
using System.Reactive;

namespace Marketplace.Api.Kafka
{
    public class CS_SyncProductRegistryInfo
    {
        private readonly ILogger<CS_SyncProductRegistryInfo> _logger;

        private readonly IMapper _mapper;

        private readonly IServiceRepository<Product> _productRepository;

        private IProducerFactory _producerFactory;

        private IConsumerFactory _consumerFactory;

        public CS_SyncProductRegistryInfo(
            IProducerFactory producerFactory,
            IConsumerFactory consumerFactory,
            IServiceRepository<Product> productRepository,
            ILogger<CS_SyncProductRegistryInfo> logger)
        {
            _producerFactory = producerFactory;
            _consumerFactory = consumerFactory;

            _productRepository = productRepository;

            _logger = logger;
        }

        [Queue("consumers")]
        public async Task ConsumeAsync()
        {
            _logger.LogInformation("[Start consuming] CS_SyncProductRegistryInfo");

            CancellationTokenSource token = new CancellationTokenSource();

            ProducerWrapper<string, SyncProductRegistryInfoAnswer>? answerProducer  = _producerFactory.Get<string, SyncProductRegistryInfoAnswer>();

            ConsumerWrapper<string, SyncProductRegistryInfoRequest>? consumer = _consumerFactory.GetSingle<string, SyncProductRegistryInfoRequest>();

            if (consumer != null)
            {
                try
                {
                    var data = consumer.ObservableData(new List<string> { "marketplace-products.syncProductRegistryInfo-req" });

                    // update marketplace product registry info
                    data.Subscribe(
                       message =>
                       {
                           _logger.LogInformation($"[{nameof(CS_SyncProductRegistryInfo)}] msgs received:");

                           Product? findProduct = _productRepository.FirstOrDefault(new GetProductByIdSpec(message.ExternalProductId ?? string.Empty));

                           if (findProduct == null)
                               _logger.LogCritical("[marketplace-products.syncProductRegistryInfo-req] product not found " + message.ExternalProductId);
                           else
                           {
                               if (answerProducer != null)
                               {
                                   answerProducer.PublishMessage(
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
                               else _logger.LogCritical("producer not found [SyncProductRegistryInfoAnswer]");
                           }

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
