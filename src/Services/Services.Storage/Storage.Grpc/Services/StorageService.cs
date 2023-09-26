using Common.Repositories;
using Grpc.Core;
using Grpc.Core.Utils;
using Grpc.Protos.Storages;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;
using Ozon.Common.Logging;
using Storage.Data.Entities.Products;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Storage.Grpc.Services
{
    public class StorageService : StorageGrpcService.StorageGrpcServiceBase
    {
        private readonly ILogger<StorageService> _logger;

        private readonly ProducerWrapper<string, List<MarketplaceProductStorageRegistrationRead>>? _productsStorageRegistrationQueue;

        private readonly IProducerFactory _producerFactory;

        public StorageService(
            IProducerFactory producerFactory,
            ILogger<StorageService> logger)
        {
            _producerFactory = producerFactory;

            _logger = logger;

            _productsStorageRegistrationQueue = producerFactory.GetBatch<string, MarketplaceProductStorageRegistrationRead>();
        }

        public override async Task<QueryStringIdResult> AddProductsToStorage(IAsyncStreamReader<AddProductToStorageRequest> requestStream, ServerCallContext context)
        {
            _logger.LogWarning(nameof(AddProductsToStorage));

            List<MarketplaceProductStorageRegistrationRead> values = new List<MarketplaceProductStorageRegistrationRead>();

            await requestStream.ForEachAsync(async (x) => values.Add(new MarketplaceProductStorageRegistrationRead
            { 
                MarketplaceProductId = x.MarketplaceProductId,
                StorageId = x.StorageId
            }));

            try
            {
                if (_productsStorageRegistrationQueue != null)
                {
                    _logger.LogInformation("[add to queue grpc-storage-registrationProducts] msgs " + values.Count);

                    // add to bus
                    _productsStorageRegistrationQueue.PublishMessage(
                        toTopicAddr: "grpc-storage-registrationProducts",
                        message: new Message<string, List<MarketplaceProductStorageRegistrationRead>>
                        {
                            Key = Guid.NewGuid().ToString(),
                            Value = values
                        });

                    
                } else _logger.LogCritical("[MarketplaceProductStorageRegistrationRead] cannot find producer" + values.Count);

                return new QueryStringIdResult
                {
                    SuccessValueId = values.First().MarketplaceProductId
                };

            }
            catch (Exception exp)
            {
                return new QueryStringIdResult
                {
                    FailureValue = new QueryErrorResult
                    {
                        ErrorMessage = exp.Message,
                        IsSuccessed = false
                    }
                };
            }
        }
    }
}
