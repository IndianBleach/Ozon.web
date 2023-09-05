using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using System.Xml.Linq;

namespace Storage.Grpc.ClickHouse
{
    public class ClickHouseStorageServiceProvider : IDisposable
    {
        private readonly ClickHouseConnection _connection;

        private const string _dbName = "StorageServiceDb";

        private const string _storageActionsTable = "StorageActions";

        private readonly string _kafkaHostname;

        private const string _storageActionsKafkaTopic = "storage_actions";

        public ClickHouseStorageServiceProvider(
            string hostName,
            uint port,
            string userName,
            string kafkaHost)
        {
            _kafkaHostname = kafkaHost;

            _connection = new ClickHouseConnection($"Host={hostName};Protocol=http;Port={port};");

            Task.Factory.StartNew(async () =>
            {
                Console.WriteLine("[init CH db]");
                await InitializeDatabaseAsync();
                Console.WriteLine("[end init CH db]");
            }).Wait();
        }

        private async Task InitializeDatabaseAsync()
        {
            // create db
            string queryDb = @$"CREATE DATABASE IF NOT EXISTS {_dbName}";
            await _connection.ExecuteScalarAsync(queryDb);

            // queue table
            string queryQueue = @$"
                CREATE TABLE {_dbName}.StorageActionsQueue (
                    storage_id UUID,
                    employee_id UUID,
                    action_id UUID,
                    action_name String,
                    storage_cell_id UInt32,
                    storage_cell_name String,
                    storage_product_id UInt32,
                    external_product_id String,
                    timestamp DateTime('Europe/Moscow')
                ) ENGINE = Kafka('{_kafkaHostname}', '{_storageActionsKafkaTopic}', 'storageActionsGroup1', 'JSONEachRow')
                    SETTINGS stream_like_engine_allow_direct_select = true";
            await _connection.ExecuteScalarAsync(queryQueue);

            // storage actions table
            string queryActionsTable = @$"
                CREATE TABLE IF NOT EXISTS {_dbName}.{_storageActionsTable} (
                   storage_id UUID,
                    employee_id UUID,
                    action_id UUID,
                    action_name String,
                    storage_cell_id UInt32,
                    storage_cell_name String,
                    storage_product_id UInt32,
                    external_product_id String,
                    timestamp DateTime('Europe/Moscow')
                ) ENGINE = MergeTree ORDER BY (timestamp)";
            await _connection.ExecuteScalarAsync(queryActionsTable);

            // сreate view
            string queryView = @$"
                CREATE MATERIALIZED VIEW {_dbName}.StorageActions_mv TO {_dbName}.{_storageActionsTable}
                AS SELECT * FROM {_dbName}.StorageActionsQueue;";
            await _connection.ExecuteScalarAsync(queryView);
        }

        public async Task SubscribeQueue(
            string queueTableName)
        {
            await _connection.ExecuteScalarAsync($"ATTACH TABLE {_dbName}.{queueTableName}");
        }

        public async Task UnsubscribeQueue(
            string queueTableName)
        {
            await _connection.ExecuteScalarAsync($"DETACH TABLE {_dbName}.{queueTableName}");
        }

        public void Dispose()
        {
            _connection.Close();
        }

        // read click-house config from ENVIRONMENT VARS
        // setup CH storageServiceDb
        // + CH storage_aciton table
        // + CH storage_events table
        // + users

        // setup CH storage_action kafka consumer
        // setup CH data provider
        // setup storageServiceGrpc with CH data provider

        // 
    }
}
