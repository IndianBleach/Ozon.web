using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus.DTOs.StorageService
{
    public class AddStorageMessage : TMessageBusValue
    {
        public int? ExternalStorageId { get; set; }

        public string? CityAddr { get; set; }

        public string? StreetAddr { get; set; }

        public string? BuildingNumberAddr { get; set; }
    }
}
