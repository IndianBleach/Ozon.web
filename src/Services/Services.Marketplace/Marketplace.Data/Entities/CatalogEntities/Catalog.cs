using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.CatalogEntities
{

    //
    

    public class Catalog : TEntity
    {
        public string Name { get; set; }

        public ICollection<CatalogCategory> Categories { get; set; }

        public Catalog(string name)
        {
            Name = name;
            Categories = new List<CatalogCategory>();
        }
    }





}
