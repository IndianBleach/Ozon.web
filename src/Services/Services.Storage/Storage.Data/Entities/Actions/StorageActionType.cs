using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Data.Entities.Actions
{
    public class StorageActionType : TEntity
    {
        public string Name { get; set; }

        public DateTime DateCreated { get; set; }

        public StorageActionType(
            string name,
            DateTime dateCreated)
        {
            Name = name;
            DateCreated = dateCreated;
        }
    }
}
