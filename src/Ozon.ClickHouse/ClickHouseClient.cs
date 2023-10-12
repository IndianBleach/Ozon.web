using ClickHouse.Client.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.ClickHouse
{
    public class ClickHouseClient
    {
        private ClickHouseConnection _connection;

        public ClickHouseClient(string connectionString)
        {
            _connection = new ClickHouseConnection(connectionString);
        }

        // execute query<T>
        // 

    }
}
