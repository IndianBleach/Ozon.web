using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus.Keys
{
    public class StringKey : TMessageBusKey
    {
        public string Key { get; set; }
    }
}
