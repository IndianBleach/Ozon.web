using Ozon.Bus.DTOs.ProductsRegistry;
using Ozon.Bus.DTOs.StorageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.BusServices
{
    public interface IMarketplaceProductBusService
    {
        //Task AddMarketplaceSeller();

        void UpdateProductInfo(
            string marketplaceProductId,
            string title,
            string description,
            double price);

        void UpdateProductSeller(
            string marketplaceProductId,
            string externalSellerId);

        void UpdateProductsStorageInfo(List<StorageProductUpdateMarketplaceStockInfo> products);
    }
}
