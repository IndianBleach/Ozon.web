using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Infrastructure.DTOs
{
    public class StorageApiCreate
    {
        public string? AddrCity { get; set; }

        public string? AddrStreet { get; set; }

        public string? AddrBuilding { get; set; }
    }
}
