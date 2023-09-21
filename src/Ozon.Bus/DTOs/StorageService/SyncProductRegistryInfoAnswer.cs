using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus.DTOs.StorageService
{
    public class SyncProductRegistryInfoAnswer
    {
        public string? MarketplaceProductId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public double DefaultPrice { get; set; }

        public string? SellerId { get; set; }
    }
}
