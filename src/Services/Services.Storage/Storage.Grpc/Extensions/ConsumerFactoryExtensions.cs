using Confluent.Kafka;
using Ozon.Bus.DTOs.ProductsRegistry;
using Ozon.Bus.DTOs.StorageService;
using Ozon.Bus;

namespace Storage.Grpc.Extensions
{
    public static class ConsumerFactoryExtensions
    {
        public static void AddConsumerFactory(
            this IServiceCollection services,
            string kafkaHost)
        {

            //services.AddSingleton<IConsumerFactory>(factory);
        }
    }
}
