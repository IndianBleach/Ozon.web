using Storage.Data.Entities.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Data.Entities.Employees
{
    public class StorageEmployee : TEntity
    {
        public string ExternalUserAccountId { get; set; }

        public int StorageId { get; set; }

        public MarketStorage? Storage { get; set; }

        public DateTime DateCreated { get; set; }

        public StorageEmployee(
            string externalUserAccountId,
            int storageId,
            DateTime dateCreated)
        {
            ExternalUserAccountId = externalUserAccountId;
            StorageId = storageId;
            DateCreated = dateCreated;
        }
    }
}
