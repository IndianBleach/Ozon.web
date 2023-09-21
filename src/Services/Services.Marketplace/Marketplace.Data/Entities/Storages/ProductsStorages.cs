using Marketplace.Data.Entities.ProductsEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.Storages
{
    public enum ProductStorageStockStatuses
    {
        IN_STOCK,
        OUT_OF_STOCK,
        ON_REGISTRATION
    }

    public class ProductsStorages : TServiceEntity
    {
        public string ProductId { get; set; }

        public CatalogProduct? Product { get; set; }

        public string InStorageId { get; set; }

        public MarketplaceProductStorage? InStorage { get; set; }

        public uint CountNow { get; set; }

        public ProductStorageStockStatuses StockStatus { get; set; }

        public DateTime ArrivedAt { get; set; }

        public ProductsStorages(
            string productId,
            string inStorageId,
            uint countNow,
            DateTime arrivedAt,
            ProductStorageStockStatuses stockStatus)
        {
            CountNow = countNow;
            StockStatus = stockStatus;
            ProductId = productId;
            InStorageId = inStorageId;
            ArrivedAt = arrivedAt;
        }
    }
}
