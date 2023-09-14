using Common.Specifications;
using Marketplace.Data.Entities.CatalogEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Infrastructure.Specifications.Sections
{
    public class AllSectionsIncludeCategorySpec : BaseSpecification<CategorySection>
    {
        public AllSectionsIncludeCategorySpec() : base(x => x.Category != null)
        {
            base.AddInclude(x => x.Category);
        }
    }
}
