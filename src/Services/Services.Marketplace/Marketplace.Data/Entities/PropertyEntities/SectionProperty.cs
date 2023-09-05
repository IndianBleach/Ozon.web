using Marketplace.Data.Entities.CatalogEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.PropertyEntities
{
    public class SectionProperty : TServiceEntity
    {
        public string SectionId { get; set; }

        public CategorySection? Section { get; set; }

        public string PropertyId { get; set; }

        public CatalogProperty? Property { get; set; }

        public bool IsRequired { get; set; }

        public SectionProperty(
            string sectionId,
            string propertyId,
            bool isRequired)
        {
            SectionId = sectionId;
            PropertyId = propertyId;
            IsRequired = isRequired;

        }
    }
}
