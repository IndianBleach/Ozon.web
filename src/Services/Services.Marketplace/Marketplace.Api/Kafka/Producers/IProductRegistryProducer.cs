namespace Marketplace.Api.Kafka.Producers
{
    public interface IProductRegistryProducer
    {
        void UpdateProductRegistryInfo(
            string productId,
            string marketplaceProductId);
    }
}
