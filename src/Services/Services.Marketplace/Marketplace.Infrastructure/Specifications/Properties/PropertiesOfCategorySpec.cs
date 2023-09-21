using Common.Specifications;
using Marketplace.Data.Entities.PropertyEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Specifications.Properties
{
    public class PropertiesOfCategorySpec : BaseSpecification<CategoryProperty>
    {
        public PropertiesOfCategorySpec(
            string categoryId,
            bool onlyRequired)
            : base(x => onlyRequired == true
                ? (x.IsRequired == true && x.CategoryId.Equals(categoryId))
                : (x.CategoryId.Equals(categoryId))
            )
        {
            base.AddInclude(x => x.Property);
            base.AddInclude(x => x.Property.Unit);
        }
    }
}
