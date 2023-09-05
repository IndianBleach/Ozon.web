using Common.Specifications;
using Storage.Data.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Specifications
{
    public class ProductsOnStorageSpec : BaseSpecification<StorageProduct>
    {
        public ProductsOnStorageSpec(int onStorageId) 
            : base(x => x.StorageId.Equals(onStorageId))
        {}
    }
}
