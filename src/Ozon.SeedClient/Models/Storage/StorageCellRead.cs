using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.SeedClient.Models.Storage
{
    public class GetAllStorageCellsResponse 
    {
        public List<StorageCellRead> Cells { get; set; }
    }

    public class StorageCellRead
    {
        public int CellId { get; set; }

        public string CellNumber { get; set; }
    }
}
