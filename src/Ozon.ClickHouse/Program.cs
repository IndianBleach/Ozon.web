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

    string queryQueue = @$"
                SELECT 
                             storage_id,
                             COUNT(storage_cell_id) AS countNow
                             FROM (SELECT 
                                    m1.storage_id,
                                    m1.storage_cell_id,
                                    (SELECT m2.action_id FROM ProductStorage.ProductMovements m2
                                    WHERE m2.storage_cell_id == m1.storage_cell_id
                                    ORDER BY m2.timestamp DESC,
                                    LIMIT 1) AS cell_state
                                  FROM ProductStorage.ProductMovements m1) AS tmp_table
                             WHERE tmp_table.cell_state = 1
                             GROUP BY storage_id";
    var ans2 = await connection.ExecuteReaderAsync(queryQueue);

    int T = 1;

}).Wait();

Console.WriteLine("[end]");