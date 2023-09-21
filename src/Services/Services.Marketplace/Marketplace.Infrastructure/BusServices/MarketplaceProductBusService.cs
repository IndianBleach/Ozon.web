using Marketplace.Data.Context;
using Marketplace.Data.Entities.Storages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ozon.Bus.DTOs.StorageService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.BusServices
{
    public class MarketplaceProductBusService : IMarketplaceProductBusService
    {
        private readonly ApplicationContext _context;

        private ILogger<MarketplaceProductBusService> _logger;

        public MarketplaceProductBusService(
            ILogger<MarketplaceProductBusService> logger,
            ApplicationContext  context)
        {
            _logger = logger;
            _context = context;

            _context.ChangeTracker.Clear();
        }

        public void UpdateProductInfo(string marketplaceProductId, string title, string description, double price)
        {
            var product = _context.CatalogProducts
                .Include(x => x.MarketplaceSeller)
                .FirstOrDefault(x => x.Id.Equals(marketplaceProductId));

            if (product != null)
            {
                product.Title = title;
                product.Description = description;
                product.Price = price;

                _context.CatalogProducts.Update(product);
                _context.SaveChanges();
            }
            else _logger.LogCritical("product not found");
        }

        public void UpdateProductSeller(string marketplaceProductId, string externalSellerId)
        {
            var product = _context.CatalogProducts
                .Include(x => x.MarketplaceSeller)
                .FirstOrDefault(x => x.Id.Equals(marketplaceProductId));

            if (product != null)
            {
                var findSeller = _context.MarketplaceSellers
                    .FirstOrDefault(x => x.ExternalSellerId.Equals(externalSellerId));

                if (findSeller != null)
                {
                    product.MarketplaceSellerId = findSeller.Id;
                    product.MarketplaceSeller = findSeller;

                    _context.CatalogProducts.Update(product);

                    _context.SaveChanges();
                }
                else _logger.LogCritical("seller not found");
            }
            else _logger.LogCritical("product not found");
        }

        public void UpdateProductsStorageInfo(List<StorageProductUpdateMarketplaceStockInfo> products)
        {
            // закешить
            // при добавлении обновить кеш
            var stors = _context.MarketplaceProductStorages.ToList();

            _logger.LogWarning("[UpdateProductsStorageInfoAsync] stors " + stors.Count);

            foreach (var group in products.GroupBy(x => new { x.MarketplaceProductId, x.StorageId}))
            {
                List<ProductsStorages> data = new List<ProductsStorages>();

                var findStore = stors.FirstOrDefault(x => x.ExternalStorageId == group.Key.StorageId);

                if (findStore == null)
                    continue;

                var existsRow = _context.ProductsStorages.FirstOrDefault(x => 
                    x.InStorageId.Equals(findStore.Id) &&
                    x.ProductId.Equals(group.Key.MarketplaceProductId));

                if (existsRow != null)
                {
                    existsRow.CountNow += (uint)group.Count();
                    _context.ProductsStorages.Update(existsRow);
                }
                else
                {
                    data.Add(new ProductsStorages(
                        productId: group.Key.MarketplaceProductId,
                        inStorageId: findStore.Id,
                        countNow: (uint)group.Count(),
                        arrivedAt: DateTime.Now,
                        stockStatus: ProductStorageStockStatuses.IN_STOCK));

                    _context.ProductsStorages.AddRange(data);
                }

                try
                {
                    _context.SaveChanges();
                }
                catch (Exception exp)
                {
                    _context.ChangeTracker.Clear();
                    _logger.LogError("[UpdateProductsStorageInfoAsync] save-error: " + exp.Message);
                }
            }
        }
    }
}
