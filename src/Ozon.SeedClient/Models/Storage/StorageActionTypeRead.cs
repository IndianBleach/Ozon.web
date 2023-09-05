using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.SeedClient.Models.Storage
{
    public class StorageActionsGetResponse
    { 
        public List<StorageActionTypeRead> Actions { get; set; }
    }

    public class StorageActionTypeRead
    {
        public int ActionId { get; set; }

        public string Name { get; set; }
    }
}
