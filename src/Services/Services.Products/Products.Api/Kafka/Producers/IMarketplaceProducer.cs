using Common.DTOs.ApiRequests.Products;

namespace Products.Api.Kafka.Producers
{
    public interface IMarketplaceProducer
    {
        void AddMarketplaceSeller(
            ProductSellerApiPost seller,
            string sellerId);
    }
}
