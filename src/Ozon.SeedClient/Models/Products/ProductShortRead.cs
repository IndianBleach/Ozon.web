using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.SeedClient.Models.Products
{
    public class GetAllProductsResponse
    { 
        public List<ProductShortRead> Products { get; set; } 
    }

    public class ProductShortRead
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string SpecialCode { get; set; }
    }
}
