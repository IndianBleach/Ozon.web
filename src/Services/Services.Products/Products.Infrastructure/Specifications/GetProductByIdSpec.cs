using Common.Specifications;
using Products.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Products.Infrastructure.Specifications
{
    public class GetProductByIdSpec : BaseSpecification<Product>
    {
        public GetProductByIdSpec(string productId) : base(x => x.Id.Equals(productId))
        {
        }
    }
}
