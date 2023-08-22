using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.CatalogEntities
{

    public class CatalogProduct
    { 
        public string ExternalProductId { get; set; }

        public string CategorySectionId { get; set; }

        public DateTime DateAdded { get; set; }
    }


    public class CategorySection
    { 
        public string CategoryId { get; set; }

        public string Name { get; set; }
    }

    public class CatalogCategory
    { 
        public string CatalogId { get; set; }

        public string Name { get; set; }

        public ICollection<CategorySection> Sections { get; set; }
    }

    public class Catalog
    {
        public string Name { get; set; }

        public ICollection<CatalogCategory> Categories { get; set; }
    }
}
