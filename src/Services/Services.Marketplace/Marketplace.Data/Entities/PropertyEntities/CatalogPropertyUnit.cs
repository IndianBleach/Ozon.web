using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.PropertyEntities
{
    public class CatalogPropertyUnit : TEntity
    {
        public string Name { get; set; }

        public CatalogPropertyUnit(string name)
        {
            Name = name;
        }
    }
}
