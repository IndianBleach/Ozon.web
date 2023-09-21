using Storage.Data.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Data.Entities.Products
{
    public class StorageProduct : TEntity
    {
        public string MarketplaceProductId { get; set; }

        public DateTime DateAdded { get; set; }

        public int StorageId { get; set; }

        public MarketStorage? Storage { get; set; }

        public StorageProduct(
            string marketplaceProductId,
            DateTime dateAdded,
            int storageId)
        {
            StorageId = storageId;
            MarketplaceProductId = marketplaceProductId;
            DateAdded = dateAdded;
        }
    }
}
