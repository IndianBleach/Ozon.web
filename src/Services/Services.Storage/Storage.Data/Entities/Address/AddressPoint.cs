using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Products.Data.Entities.Address
{
    public class AddressPoint : TEntity
    {
        public string CityAddr { get; set; }

        public string StreetAddr { get; set; }

        public string BuildingNumberAddr { get; set; }

        public AddressPoint(
            string cityAddr,
            string streetAddr,
            string buildingNumberAddr)
        {
            CityAddr = cityAddr;
            StreetAddr = streetAddr;
            BuildingNumberAddr = buildingNumberAddr;
        }
    }
}
