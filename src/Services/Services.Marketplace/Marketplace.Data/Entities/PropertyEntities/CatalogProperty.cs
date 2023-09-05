using Marketplace.Data.Entities.ProductsEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.PropertyEntities
{
    public class CatalogProperty : TEntity
    {
        public string Name { get; set; }

        public string UnitId { get; set; }

        public CatalogPropertyUnit? Unit { get; set; }

        public string? Description { get; set; }

        public ICollection<ProductPropertyValue> ProductsValues { get; set; }

        public ICollection<SectionProperty> InSections { get; set; }

        public ICollection<CategoryProperty> InCategories { get; set; }

        public CatalogProperty(
            string name,
            string unitId,
            string? description)
        {
            Name = name;
            UnitId = unitId;
            Description = description;
            ProductsValues = new List<ProductPropertyValue>();
            InSections = new List<SectionProperty>();
            InCategories = new List<CategoryProperty>();
        }

    }
}
