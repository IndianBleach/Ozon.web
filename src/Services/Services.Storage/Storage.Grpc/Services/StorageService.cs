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

namespace Storage.Grpc.Services
{
    public class StorageService : StorageGrpcService.StorageGrpcServiceBase
    {
        private readonly ILogger<StorageService> _logger;

        private readonly Producer<string, List<MarketplaceProductStorageRegistrationRead>> _productsStorageRegistrationQueue;

        public StorageService(
            ILogger<StorageService> logger)
        {
            _logger = logger;

            _productsStorageRegistrationQueue = new Producer<string, List<MarketplaceProductStorageRegistrationRead>>(
                config: new ProducerConfig()
                {
                    BootstrapServers = "kafka-broker:9092",
                    Acks = Acks.Leader,
                    EnableBackgroundPoll = false,
                });
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
                _logger.LogCritical("[add to queue grpc-storage-registrationProducts] msgs " + values.Count);

                // add to bus
                _productsStorageRegistrationQueue.PublishMessage(
                    toTopicAddr: "grpc-storage-registrationProducts",
                    message: new Message<string, List<MarketplaceProductStorageRegistrationRead>>
                    {
                        Key = Guid.NewGuid().ToString(),
                        Value = values
                    });

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
