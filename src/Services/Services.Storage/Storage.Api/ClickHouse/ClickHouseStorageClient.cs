using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Common.DataQueries;
using Common.DTOs.Storage.ClickHouse;
using Hangfire.MemoryStorage.Database;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client.Extensions.Msal;

namespace Storage.Api.ClickHouse
{
    internal class ClickHouseStorageClient : IClickHouseStorageClient
    {
        private ClickHouseConnection _connection;

        private ClickHouseStorageServiceOptions _options;

        private ILogger<ClickHouseStorageClient> _logger;

        public ClickHouseStorageClient(
            ILogger<ClickHouseStorageClient> logger,
            ClickHouseStorageServiceOptions options,
            string connectionString)
        {
            _options = options;

            _logger = logger;

            Console.WriteLine($"initialize Click-House: {_options.KafkaHost} {_options.KafkaPort} {_options.ProductMovemementsTopic}");

            _connection = new ClickHouseConnection(connectionString);

            _initializeDatabase();

            Console.WriteLine("INIT CH (end)");
        }

        // refact
        private void _initializeDatabase()
        {
            #region ProductMovements_kafkaEngineQueue

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    await _connection.ExecuteScalarAsync("CREATE DATABASE IF NOT EXISTS ProductStorage");

                    string queryQueue = @$"
                CREATE TABLE IF NOT EXISTS ProductStorage.ProductMovements_Queue (
                    storage_id Int32,
                    action_id Int32,
                    action_name String,
                    storage_cell_id Int32,
                    storage_cell_name String,
                    storage_product_id Int32,
                    marketplace_product_id UUID
                ) ENGINE = Kafka()
                SETTINGS
                    kafka_broker_list = 'kafka-broker:9092',
                    kafka_topic_list = '{_options.ProductMovemementsTopic}',
                    kafka_group_name = 'listener_ProductMovements_ch',
                    kafka_format = 'JSONEachRow',
                    kafka_commit_every_batch = 0,
                    kafka_poll_timeout_ms = 1000,
                    kafka_commit_on_select = false";
                    
                    await _connection.ExecuteScalarAsync(queryQueue);

                    Console.WriteLine("[ch step 1]");

                    string queryActionsTable = @$"
                CREATE TABLE IF NOT EXISTS ProductStorage.ProductMovements (
                    storage_id Int32,
                    action_id Int32,
                    action_name String,
                    storage_cell_id Int32,
                    storage_cell_name String,
                    storage_product_id Int32,
                    marketplace_product_id UUID,
                    timestamp DateTime DEFAULT now(),
                    PRIMARY KEY (storage_cell_id, storage_product_id, action_id)
                ) ENGINE = MergeTree ORDER BY (storage_cell_id, storage_product_id, action_id)";
                    await _connection.ExecuteScalarAsync(queryActionsTable);

                    Console.WriteLine("[ch step 2]");

                    string queryView = @$"
                CREATE MATERIALIZED VIEW IF NOT EXISTS ProductStorage.ProductMovements_MaterialView TO ProductStorage.ProductMovements
                AS SELECT * FROM ProductStorage.ProductMovements_Queue;";
                    await _connection.ExecuteScalarAsync(queryView);


                    _logger.LogInformation("[StorageClickHouseClient] database initialized");
                }
                catch (Exception exp)
                {
                    _logger.LogCritical("error with init ClickHouseDatabase: " + exp.Message);
                }

            }).Wait();

            Console.WriteLine("end of wait");

            #endregion
        }

        public async Task<StorageProductMovementRead[]> GetHistoryOfStorageCell(int storageId, int cellId)
        {
            string query = string.Format(@"SELECT 
                             storage_id,
                             action_id,
                             action_name,
                             storage_cell_id,
                             storage_cell_name,
                             storage_product_id,
                             marketplace_product_id,
                             timestamp
                      FROM ProductStorage.ProductMovements
                      WHERE storage_id = {0} AND storage_cell_id = {1}
                      ORDER BY timestamp DESC", storageId, cellId);

            var reader = await _connection.ExecuteReaderAsync(
                sql: query);

            List<StorageProductMovementRead> data = new List<StorageProductMovementRead>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    data.Add(new StorageProductMovementRead(
                        storageId: storageId,
                        actionId: reader.GetInt32(0),
                        actionName: reader.GetString(1),
                        cellId: reader.GetInt32(2),
                        cellName: reader.GetString(3),
                        storageProductId: reader.GetInt32(4),
                        marketplaceProductId: reader.GetGuid(5).ToString(),
                        timestamp: reader.GetDateTime(6)));
                }
            }

            return data.ToArray();
        }

        public async Task<QueryResult<StorageProductMovementRead>> GetProductStorageLocationNow(int storageProductId)
        {
            string query = string.Format(@"SELECT 
                             storage_id,
                             action_id,
                             action_name,
                             storage_cell_id,
                             storage_cell_name,
                             storage_product_id,
                             marketplace_product_id,
                             timestamp
                      FROM ProductStorage.ProductMovements
                      WHERE storage_product_id = {0}
                      ORDER BY timestamp DESC
                      LIMIT 0,1", storageProductId);

            var reader = await _connection.ExecuteReaderAsync(
                sql: query);

            if (reader.HasRows && reader.Read())
            {
                StorageProductMovementRead result = new StorageProductMovementRead(
                        storageId: reader.GetInt32(0),
                        actionId: reader.GetInt32(1),
                        actionName: reader.GetString(2),
                        cellId: reader.GetInt32(3),
                        cellName: reader.GetString(4),
                        storageProductId: reader.GetInt32(5),
                        marketplaceProductId: reader.GetGuid(6).ToString(),
                        timestamp: reader.GetDateTime(7));

                return QueryResult<StorageProductMovementRead>.Successed(result);
            }
            else return QueryResult<StorageProductMovementRead>.Failure("product not found");
        }

        public async Task<PagedList<StorageProductMovementRead>> GetPagedProductsMovements(int page)
        {
            int takeRows = 15;

            string query = string.Format(@"SELECT 
                             storage_id,
                             action_id,
                             action_name,
                             storage_cell_id,
                             storage_cell_name,
                             storage_product_id,
                             marketplace_product_id,
                             timestamp
                      FROM ProductStorage.ProductMovements
                      LIMIT {0},{1}", ((page - 1) * takeRows), takeRows);

            object totalObj = await _connection.ExecuteScalarAsync(
                "SELECT count() FROM ProductStorage.ProductMovements");
            int total = Convert.ToInt32(totalObj);


            var reader = await _connection.ExecuteReaderAsync(
                sql: query);

            List<StorageProductMovementRead> data = new List<StorageProductMovementRead>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    data.Add(new StorageProductMovementRead(
                        storageId: reader.GetInt32(0),
                        actionId: reader.GetInt32(1),
                        actionName: reader.GetString(2),
                        cellId: reader.GetInt32(3),
                        cellName: reader.GetString(4),
                        storageProductId: reader.GetInt32(5),
                        marketplaceProductId: reader.GetGuid(6).ToString(),
                        timestamp: reader.GetDateTime(7)));
                }
            }

            return PagedList<StorageProductMovementRead>.GeneratePages(total, page, data);
        }

        public async Task<MarketProductVariantStorageSummary[]> GetMarketplaceProductStoragesSummary(string marketplaceProductVariantId)
        {
            int actionPutId = 1;

            /* result query
                storage_id	countNow
                1	        2
                2	        2
                3	        1
             */

            string query = string.Format(@"
                SELECT
                    m1.storage_id as storage_id,
                    COUNT(m1.storage_cell_id) as countNow
                    FROM ProductStorage.ProductMovements m1
                    JOIN (SELECT m2.marketplace_product_id as prod_id, m2.action_id as cell_state, m2.storage_cell_id as cell_id 
                        FROM ProductStorage.ProductMovements m2
                        WHERE prod_id = toUUID('{0}')
                        ORDER BY m2.timestamp DESC
                        LIMIT 1) AS subq
                    ON (m1.storage_cell_id = subq.cell_id)
                    WHERE subq.cell_state = {1}
                    GROUP BY storage_id", marketplaceProductVariantId, actionPutId);

            var reader = await _connection.ExecuteReaderAsync(
                sql: query);

            List<MarketProductVariantStorageSummary> data = new List<MarketProductVariantStorageSummary>();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    data.Add(new MarketProductVariantStorageSummary(
                        storageId: reader.GetInt32(0),
                        marketplaceProductVariantId: marketplaceProductVariantId,
                        countAvailableNow: (uint)reader.GetInt32(1)));
                }
            }

            return data.ToArray();
        }
    }
}
