using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.CatalogEntities
{
    public class CategorySection : TEntity
    {
        public string CategoryId { get; set; }

        public CatalogCategory? Category { get; set; }

        public string Name { get; set; }

        public CategorySection(
            string categoryId,
            string name)
        {
            CategoryId = categoryId;
            Name = name;
        }
    }
}
