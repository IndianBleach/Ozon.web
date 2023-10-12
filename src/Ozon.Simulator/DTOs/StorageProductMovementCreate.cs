using Newtonsoft.Json;
using Ozon.Bus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Simulator.DTOs
{
    public class StorageProductMovementCreate
    {
        [JsonProperty]
        public Int32 storage_id { get; set; }

        [JsonProperty]
        public Int32 action_id { get; set; }

        [JsonProperty]
        public string action_name { get; set; }

        [JsonProperty]
        public Int32 storage_cell_id { get; set; }

        [JsonProperty]
        public string storage_cell_name { get; set; }

        [JsonProperty]
        public Int32 storage_product_id { get; set; }

        [JsonProperty]
        public string marketplace_product_id { get; set; }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

        public StorageProductMovementCreate(
            Int32 storageId,
            Int32 actionId,
            string actionName,
            Int32 cellId,
            string cellName,
            Int32 storageProductId,
            string marketplaceProductId)
        {
            marketplace_product_id = marketplaceProductId;
            storage_product_id = storageProductId;
            storage_cell_id = cellId;
            storage_cell_name = cellName;
            action_name = actionName;
            storage_id = storageId;
            action_id = actionId;
        }
    }
}
