using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus.DTOs.ProductsRegistry
{
    public class ProductRegistryMarketplaceSeller : TMessageBusValue
    {
        public string? ExternalSellerId { get; set; }

        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? Site { get; set; }

        public string? Description { get; set; }
    }
}
