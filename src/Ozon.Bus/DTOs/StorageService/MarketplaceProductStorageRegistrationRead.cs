using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus.DTOs.StorageService
{
    public class MarketplaceProductStorageRegistrationRead
    {
        public string? MarketplaceProductId { get; set; }

        public int StorageId { get; set; }
    }
}
