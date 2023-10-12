// See https://aka.ms/new-console-template for more information
using Ozon.Simulator.Simulators;
using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Data;
using Common.DTOs.Storage.ClickHouse;
using Ozon.Simulator.Streams;

Console.WriteLine("Hello, World!");


#region Streams

StorageServiceStream storageStream = new StorageServiceStream();

Task.Run(async () =>
{
    var t1 = storageStream.Stream_MarketProductsStorageRegistrations(100);
    var t2 = storageStream.Stream_MarketProductsStorageRegistrations(100);
    var t3 = storageStream.Stream_MarketProductsStorageRegistrations(100);

    Task.WaitAll(t1, t2, t3);
}).Wait();

#endregion


List<StorageProductMovementRead> data1 = new List<StorageProductMovementRead>();

try
{
    var _connection = new ClickHouseConnection("Host=localhost;Protocol=http;Database=default;Port=8123;Username=default;");

    // select storage_cell
    // order by timestamp desc
    // distinct by storage_cell
    // limit 0,1
    // where action_name = положить на ячейку and marketplace_product_id = prodId

    string query = string.Format(@"SELECT 
                             storage_id,
                             storage_product_id,
                             marketplace_product_id,
                             timestamp
                      FROM ProductStorage.ProductMovements
                      WHERE storage_product_id = 12
                      ORDER BY timestamp DESC
                      LIMIT 0,1", 0);

    var reader = await _connection.ExecuteReaderAsync(
        sql: query);

    //if (reader.HasRows && reader.Read())
    //{
    //    StorageProductLocationRead result = new StorageProductLocationRead(
    //        ,);
    //}
}
catch (Exception exp)
{
    Console.WriteLine("error CH: " + exp.Message);
}


StorageSimulator storageSimulator = new StorageSimulator();


var t1 = storageSimulator.SimulateProductsMovements(200, Simulator_ProductMovementsActions.PutToCell);
var t2 = storageSimulator.SimulateProductsMovements(200, Simulator_ProductMovementsActions.PutToCell);
var t3 = storageSimulator.SimulateProductsMovements(200, Simulator_ProductMovementsActions.GetFromCell);

Task.WaitAll(t1, t2, t3);


Console.WriteLine("end stream");