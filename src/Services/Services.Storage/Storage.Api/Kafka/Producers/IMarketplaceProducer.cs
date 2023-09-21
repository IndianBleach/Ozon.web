namespace Storage.Api.Kafka.Producers
{
    public interface IMarketplaceProducer
    {
        void AddMarketplaceStorage(
            int externalStorageId,
            string city,
            string street,
            string building);
    }
}
