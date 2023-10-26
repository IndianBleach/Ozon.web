using Confluent.Kafka;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;

namespace Storage.Api.Extensions
{
    public static class ProducerFactoryExtensions
    {
        public static void AddProducerFactory(
            this IServiceCollection services,
            string kafkaHost)
        {
            var producerFactory = new ProducerFactory();
            producerFactory.Register<string, StorageProductUpdateMarketplaceStockInfo>(new ProducerConfig()
            {
                BootstrapServers = kafkaHost,
                Acks = Acks.Leader,
                EnableBackgroundPoll = false,
            });

            producerFactory.Register<string, SyncProductRegistryInfoAnswer>(new ProducerConfig()
            {
                BootstrapServers = kafkaHost,
                Acks = Acks.Leader,
                EnableBackgroundPoll = false
            });

            #region Upd-1

            producerFactory.Register<string, AddStorageMessage>(new ProducerConfig()
            {
                BootstrapServers = kafkaHost,
                Acks = Acks.Leader,
                EnableBackgroundPoll = false,
                MessageSendMaxRetries = 3,
                RetryBackoffMs = 2000,
                RequestTimeoutMs = 4000,
                ReconnectBackoffMaxMs = 4000
            });

            #endregion

            services.AddSingleton<IProducerFactory>(producerFactory);
        }
    }
}
