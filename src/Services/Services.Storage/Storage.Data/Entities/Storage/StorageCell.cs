using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Data.Entities.Storage
{
    public class StorageCell : TEntity
    {
        public string CellNumber { get; set; }

        public string? Commentary { get; set; }

        public int StorageId { get; set; }

        public MarketStorage? Storage { get; set; }

        public StorageCell(
            string cellNumber,
            string? commentary,
            int storageId)
        {
            CellNumber = cellNumber;
            Commentary = commentary;
            StorageId = storageId;
        }
    }
}
