using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.SeedClient.Models.Storage
{
    public class GetAllStoragesResponse {
        public List<StorageShortRead> Storages { get; set; }
    }

    public class StorageShortRead
    {
        public int StorageId { get; set; }
    }
}
