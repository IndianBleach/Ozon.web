using Confluent.Kafka;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;

namespace Storage.Grpc.Extensions
{
    public static class ProducerFactoryExtensions
    {
        public static void AddProducerFactory(
            this IServiceCollection services,
            string kafkaHost)
        {
            var producerFactory = new ProducerFactory();
            producerFactory.Register<string, MarketplaceProductStorageRegistrationRead>(new ProducerConfig()
            {
                BootstrapServers = kafkaHost,
                Acks = Acks.Leader,
                EnableBackgroundPoll = false,
            });

            services.AddSingleton<IProducerFactory>(producerFactory);
        }
    }
}
