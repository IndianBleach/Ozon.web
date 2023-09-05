using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.SeedClient.Models.Storage
{
    public class GetStorageProductsResponse
    {
        public int StorageId { get; set; }

        public List<StorageProductRead> Products { get; set; }
    }

    public class StorageProductRead
    {
        public string ExternalProductId { get; set; }

        public int InStorageProductId { get; set; }
    }
}
