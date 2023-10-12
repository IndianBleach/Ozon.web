using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus.DTOs.StorageService
{
    public class StorageProductUpdateMarketplaceStockInfo : TMessageBusValue
    {
        public string? MarketplaceProductId { get; set; }

        public string? NewStockStatusName { get; set; }

        public int StorageId { get; set; }
    }
}
