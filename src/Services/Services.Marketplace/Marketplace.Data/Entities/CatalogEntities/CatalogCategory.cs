using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.CatalogEntities
{
    public class CatalogCategory : TEntity
    {
        public string CatalogId { get; set; }

        public Catalog? Catalog { get; set; }

        public string Name { get; set; }

        public ICollection<CategorySection> Sections { get; set; }

        public CatalogCategory(
            string catalogId,
            string name)
        {
            CatalogId = catalogId;
            Name = name;
            Sections = new List<CategorySection>();
        }
    }
}
