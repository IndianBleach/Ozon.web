using Common.DataQueries;
using Common.DTOs.Storage.ClickHouse;

namespace Storage.Api.ClickHouse
{
    public interface IClickHouseStorageClient
    {
        Task<StorageProductMovementRead[]> GetHistoryOfStorageCell(
            int storageId,
            int cellId);

        Task<QueryResult<StorageProductMovementRead>> GetProductStorageLocationNow(
            int storageProductId);

        Task<PagedList<StorageProductMovementRead>> GetPagedProductsMovements(
            int page);

        Task<MarketProductVariantStorageSummary[]> GetMarketplaceProductStoragesSummary(string marketplaceProductVariantId);
    }
}
