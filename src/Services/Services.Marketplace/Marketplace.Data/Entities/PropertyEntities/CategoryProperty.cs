using Marketplace.Data.Entities.CatalogEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.PropertyEntities
{
    public class CategoryProperty : TServiceEntity
    {
        public string CategoryId { get; set; }

        public CatalogCategory? Category { get; set; }

        public string PropertyId { get; set; }

        public CatalogProperty? Property { get; set; }

        public bool IsRequired { get; set; }

        public CategoryProperty(
            string categoryId,
            string propertyId,
            bool isRequired)
        {
            CategoryId = categoryId;
            PropertyId = propertyId;
            IsRequired = isRequired;

        }
    }
}
