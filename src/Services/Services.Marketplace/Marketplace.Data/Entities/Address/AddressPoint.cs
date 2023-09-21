using Marketplace.Data.Entities.Storages;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.Data.Entities.Address
{
    public class AddressPoint : TEntity
    {
        public string City { get; set; }

        public string Street { get; set; }

        public string BuildingNumber { get; set; }

        public ICollection<MarketplaceProductStorage> MarketplaceStorages { get; set; }

        public AddressPoint(
            string city,
            string street,
            string buildingNumber)
        {
            City = city;
            Street = street;
            BuildingNumber = buildingNumber;
            MarketplaceStorages = new List<MarketplaceProductStorage>();
        }
    }
}
