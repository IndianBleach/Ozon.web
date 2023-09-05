using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.SeedClient.Models.Storage
{
    public class GetStorageEmployeesResponse
    { 
        public List<StorageEmployeeRead> Employees { get; set; }
    }

    public class StorageEmployeeRead
    {
        public int StorageId { get; set; }

        public string UserAccountId { get; set; }

        public int StorageEmployeeId { get; set; }
    }
}
