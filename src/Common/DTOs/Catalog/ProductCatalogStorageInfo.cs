using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs.Catalog
{
    public class ProductCatalogStorageInfo
    {
        public string Id { get; set; }

        public string FullAddr { get; set; }

        public string StockStatusName { get; set; }

        public uint CountNow { get; set; }

        public ProductCatalogStorageInfo(
            string id,
            string fullAddr,
            string stockStatusName,
            uint countNow)
        {
            Id = id;
            FullAddr = fullAddr;
            StockStatusName = stockStatusName;
            CountNow = countNow;
        }
    }
}
