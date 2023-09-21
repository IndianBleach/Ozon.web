using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus.DTOs.StorageService
{
    public class SyncProductRegistryInfoRequest
    {
        public string? ExternalProductId { get; set; }

        public string? MarketplaceProductId { get; set; }

        public string? BusAsnwerChannel { get; set; }
    }
}
