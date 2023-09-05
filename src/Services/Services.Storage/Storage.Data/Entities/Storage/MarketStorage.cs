using Storage.Data.Entities.Address;
using Storage.Data.Entities.Employees;
using Storage.Data.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Data.Entities.Storage
{
    public class MarketStorage : TEntity
    {
        public int AddressId { get; set; }

        public AddressPoint? Address { get; set; }

        public ICollection<StorageCell> StorageCells { get; set; }

        public ICollection<StorageEmployee> StorageEmployees { get; set; }

        public ICollection<StorageProduct> StorageProducts { get; set; }

        public MarketStorage(int addressId)
        {
            AddressId = addressId;
            StorageCells = new List<StorageCell>();
            StorageEmployees = new List<StorageEmployee>();
            StorageProducts = new List<StorageProduct>();
        }
    }
}
