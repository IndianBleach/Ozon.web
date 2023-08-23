using Products.Data.Entities.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data.Entities.Storage
{
    public class MarketStorage : TEntity
    {
        public string AddressId { get; set; }

        public AddressPoint? Address { get; set; }

        public ICollection<StorageCell> StorageCells { get; set; }

        public MarketStorage(string addressId)
        {
            AddressId = addressId;
            StorageCells = new List<StorageCell>();
        }
    }
}
