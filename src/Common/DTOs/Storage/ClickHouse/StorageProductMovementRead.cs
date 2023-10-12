using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTOs.Storage.ClickHouse
{
    public class StorageProductMovementRead
    {
        public int StorageId { get; set; }

        public int ActionId { get; set; }

        public string ActionName { get; set; }

        public int StorageCellId { get; set; }

        public string StorageCellName { get; set; }

        public int StorageProductId { get; set; }

        public string MarketplaceProductId { get; set; }

        public DateTime Timestamp { get; set; }

        public StorageProductMovementRead(
            int storageId,
            int actionId,
            string actionName,
            int cellId,
            string cellName,
            int storageProductId,
            string marketplaceProductId,
            DateTime timestamp)
        {
            StorageId = storageId;
            ActionId = actionId;
            ActionName = actionName;
            StorageCellId = cellId;
            StorageCellName = cellName;
            StorageProductId = storageProductId;
            MarketplaceProductId = marketplaceProductId;
            Timestamp = timestamp;

        }
    }
}
