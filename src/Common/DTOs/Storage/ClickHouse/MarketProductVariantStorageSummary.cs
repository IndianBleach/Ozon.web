using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs.Storage.ClickHouse
{
    public class MarketProductVariantStorageSummary
    {
        public int StorageId { get; set; }

        public string MarketplaceProductVariantId { get; set; }

        public uint CountAvailableNow { get; set; }

        public MarketProductVariantStorageSummary(
            int storageId,
            string marketplaceProductVariantId,
            uint countAvailableNow)
        {
            StorageId = storageId;
            MarketplaceProductVariantId = marketplaceProductVariantId;
            CountAvailableNow = countAvailableNow;
        }
    }
}
