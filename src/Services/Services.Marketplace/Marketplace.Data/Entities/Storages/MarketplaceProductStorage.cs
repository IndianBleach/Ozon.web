using Marketplace.Data.Entities.Address;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.Storages
{
    public class MarketplaceProductStorage : TEntity
    {
        // add user-favorite storages
        public int ExternalStorageId { get; set; }

        public string StorageAddrId { get; set; }

        public AddressPoint? StorageAddr { get; set; }

        public bool Verified { get; set; }

        public MarketplaceProductStorage(
            int externalStorageId,
            string storageAddrId,
            bool verified)
        {
            ExternalStorageId = externalStorageId;
            StorageAddrId = storageAddrId;
            Verified = verified;

        }

    }
}
