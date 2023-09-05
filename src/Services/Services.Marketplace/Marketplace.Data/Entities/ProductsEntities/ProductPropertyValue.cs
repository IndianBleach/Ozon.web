using Marketplace.Data.Entities.PropertyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.ProductsEntities
{
    public class ProductPropertyValue : TServiceEntity
    {
        public string CatalogProductId { get; set; }

        public CatalogProduct? CatalogProduct { get; set; }

        public string PropertyId { get; set; }

        public CatalogProperty? Property { get; set; }

        public string Value { get; set; }

        public ProductPropertyValue(
            string catalogProductId,
            string propertyId,
            string value)
        {
            CatalogProductId = catalogProductId;
            PropertyId = propertyId;
            Value = value;
        }
    }
}
