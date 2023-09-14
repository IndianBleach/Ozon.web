using Common.Specifications;
using Marketplace.Data.Entities.PropertyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Specifications.Properties
{
    public class PropertiesOfSectionSpec : BaseSpecification<SectionProperty>
    {
        public PropertiesOfSectionSpec(
            string sectionId,
            bool onlyRequired)
            : base(x => onlyRequired == true
                ? (x.IsRequired == true && x.SectionId.Equals(sectionId))
                : (x.SectionId.Equals(sectionId))
            )
        {
            base.AddInclude(x => x.Property);
            base.AddInclude(x => x.Property.Unit);
        }
    }
}
