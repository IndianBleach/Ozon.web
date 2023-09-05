using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Kafka
{
    /*
     * 
     * storage_id UUID,
       employee_id UUID,
       action_id UUID,
       timestamp DateTime('Europe/Moscow')
     * 
     */


    public class StorageActionKafkaRead
    {
        [JsonProperty]
        public Int32 storage_id { get; set; }

        [JsonProperty]
        public Int32 employee_id { get; set; }

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
        public string external_product_id { get; set; }

        public StorageActionKafkaRead(
            Int32 storageId,
            Int32 employeeId,
            Int32 actionId,
            string actionName,
            Int32 cellId,
            string cellName,
            Int32 storageProductId,
            string externalProductId)
        {
            Console.WriteLine("[ctor2]");
            external_product_id = externalProductId;
            storage_product_id = storageProductId;
            storage_cell_id = cellId; 
            storage_cell_name = cellName;
            action_name = actionName;
            storage_id = storageId;
            employee_id = employeeId;
            action_id = actionId;
        }
    }
}
