using Marketplace.Data.Entities.CatalogEntities;
using Marketplace.Data.Entities.Sellers;
using Marketplace.Data.Entities.Storages;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.ProductsEntities
{
    public enum ProductCatalogStatuses
    { 
        ON_REGISTRATION,
        REGISTERED
    }

    public class CatalogProduct : TEntity
    {
        public string ExternalBaseProductId { get; set; }

        public string CategorySectionId { get; set; }

        public CategorySection? CategorySection { get; set; }

        public DateTime DateAdded { get; set; }

        public ICollection<ProductPropertyValue> Properties { get; set; }

        // sync
        public string? Title { get; set; }

        // sync
        public string? Description { get; set; }

        // sync (serv.products)
        public ProductCatalogStatuses CatalogStatus { get; set; }

        // sync (serv.products)
        public double? Price { get; set; }

        // sync (serv.products)
        public string? MarketplaceSellerId { get; set; }

        // sync (serv.products)
        public MarketplaceSeller? MarketplaceSeller { get; set; }

        public ICollection<ProductsStorages> AvailableInStorages { get; set; }

        public CatalogProduct(
            string externalBaseProductId,
            string categorySectionId,
            string? title,
            double? price,
            string? marketplaceSellerId,
            string? description,
            ProductCatalogStatuses catalogStatus,
            DateTime dateAdded)
        {
            Title = title;
            Description = description;
            Price = price;
            MarketplaceSellerId = marketplaceSellerId;
            ExternalBaseProductId = externalBaseProductId;
            CategorySectionId = categorySectionId;
            CatalogStatus = catalogStatus;
            DateAdded = dateAdded;
            Properties = new List<ProductPropertyValue>();
            AvailableInStorages = new List<ProductsStorages>();
        }
    }
}
