using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs.Catalog
{
    public class CatalogPropertyValueRead
    { 
        public CatalogPropertyRead Property { get; set; }

        public string Value { get; set; }
    }

    public class CatalogProductVariantRead
    { 
        public string ProductId { get; set; }

        public CatalogPropertyValueRead PropertyValue { get; set; }
    }

    public class CatalogProductRead
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public CatalogCategoryRead Category { get; set; }

        public CatalogRead Catalog { get; set; }

        public CatalogSectionRead Section { get; set; }

        public string Price { get; set; }

        public CatalogProductVariantRead[] Variants { get; set; }
    }
}
