using Marketplace.Data.Entities.CatalogEntities;
using Marketplace.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.ProductsEntities
{
    public class CatalogProduct : TEntity
    {
        public string ExternalBaseProductId { get; set; }

        public string CategorySectionId { get; set; }

        public CategorySection? CategorySection { get; set; }

        public ProductCatalogStatuses? CatalogStatus { get; set; }

        public DateTime DateAdded { get; set; }

        public ICollection<ProductPropertyValue> Properties { get; set; }

        public CatalogProduct(
            string externalBaseProductId,
            string categorySectionId,
            ProductCatalogStatuses catalogStatus,
            DateTime dateAdded)
        {
            ExternalBaseProductId = externalBaseProductId;
            CategorySectionId = categorySectionId;
            CatalogStatus = catalogStatus;
            DateAdded = dateAdded;
            Properties = new List<ProductPropertyValue>();
        }

    }
}
