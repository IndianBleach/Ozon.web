using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs.Storage
{
    public class StorageProductRead
    {
        public string MarketplaceProductId { get; set; }

        public int StorageProductId { get; set; }

        public int InStorageId { get; set; }
    }
}
