using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data.Entities.Storage
{
    public class StorageCell : TEntity
    {
        public string CellNumber { get; set; }

        public string? Commentary { get; set; }

        public string StorageId { get; set; }

        public MarketStorage? Storage { get; set; }

        public StorageCell(
            string cellNumber,
            string? commentary,
            string storageId)
        {
            CellNumber = cellNumber;
            Commentary = commentary;
            StorageId = storageId;
        }
    }
}
