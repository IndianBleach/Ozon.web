// See https://aka.ms/new-console-template for more information


using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using System.Data;
using System.Data.Common;

/* сид товаров
    1. регистрация нового товара (products)
    2. отправление на склад (storage)
    3. распределение его по ячейкам
    4. товар доступен в магазине
    5. заказ (basket)
    6. сборка заказа на складе (storage ch)
    7. отправление в доставку (обновление статуса заказа в basket)
 */

// TODO: добавлять в storage_product marketplaceProductId и externalBaseProductId

// seed store_actions +
// read store_actions queries
// add store metrics (grafana + prometheus)

// помимо click-house перехватить сообщения! закомиттить

// как пришел на склад:
//  -> сообщение в маркетплейс о доступности товара
//  -> сообщение и изменение статуса в services.products

// начать описывать маркетплейс
//  -> продукт (статусы)

// начать описывать модель продукта для вывода в маркеплейсе

// описать сборку заказа
// 1. заказ товара (кол-во)
// 2. сборка на складе (с поиском в ch)
// 3. таблица readyToDelivery
// 4. после сборки обновить статус заказа (статусы в services.basket)




Task.Run(async () =>
{
    var connection = new ClickHouseConnection($"Host=localhost;Protocol=http;Port=8123;");

    DataTable ans1 = connection.ExecuteDataTable("select * from default.Actions7");

    int s = 0;
    foreach (DataColumn item in ans1.Columns)
    {
        Console.Write(item.ColumnName + " ");
    }

    foreach (DataRow item in ans1.Rows)
    {
        foreach (var cell in item.ItemArray)
        {
            Console.Write(cell + " ");
        }
        Console.WriteLine("");
    }

    //var ans = await connection.ExecuteScalarAsync("SET input_format_import_nested_json=1");

    //string queryQueue = @$"
    //            CREATE TABLE default.queue9 (
    //                Number Int32,
    //                Name String
    //            ) ENGINE = Kafka('kafka-broker:9092', 'juice', 'g{Guid.NewGuid()}', 'JSONEachRow')";
    //var ans2 = await connection.ExecuteScalarAsync(queryQueue);

    //string queryActionsTable = @$"
    //            CREATE TABLE IF NOT EXISTS default.store9 (
    //                Number Int32,
    //                Name String
    //            ) ENGINE = MergeTree ORDER BY (Number)";
    //var ans3 = await connection.ExecuteScalarAsync(queryActionsTable);

    //string queryView = @$"
    //            CREATE MATERIALIZED VIEW default.mv9 TO default.store9
    //            AS SELECT * FROM default.queue9;";
    //var ans4 = await connection.ExecuteScalarAsync(queryView);


    //#region Init
    //string queryQueue = @$"
    //            CREATE TABLE default.ActionsQueue7 (
    //                storage_id Int32,
    //                employee_id Int32,
    //                action_id Int32,
    //                action_name String,
    //                storage_cell_id Int32,
    //                storage_cell_name String,
    //                storage_product_id Int32,
    //                external_product_id UUID
    //            ) ENGINE = Kafka('kafka-broker:9092', 'chance', 'cons{Guid.NewGuid()}', 'JSONEachRow')";
    //var ans2 = await connection.ExecuteScalarAsync(queryQueue);

    //string queryActionsTable = @$"
    //            CREATE TABLE IF NOT EXISTS default.Actions7 (
    //                storage_id Int32,
    //                employee_id Int32,
    //                action_id Int32,
    //                action_name String,
    //                storage_cell_id Int32,
    //                storage_cell_name String,
    //                storage_product_id Int32,
    //                external_product_id UUID
    //            ) ENGINE = MergeTree ORDER BY (action_id)";
    //var ans3 = await connection.ExecuteScalarAsync(queryActionsTable);

    //string queryView = @$"
    //            CREATE MATERIALIZED VIEW default.actions_mv7 TO default.Actions7
    //            AS SELECT * FROM default.ActionsQueue7;";
    //var ans4 = await connection.ExecuteScalarAsync(queryView);

    int t = 1;
    //#endregion



}).Wait();

Console.WriteLine("[end]");