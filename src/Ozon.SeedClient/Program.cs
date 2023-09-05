

using ClickHouse.Client.ADO;
using ClickHouse.Client.Utility;
using Common.Kafka;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NJsonSchema.Generation;
using NodaTime;
using Ozon.SeedClient.Models.Products;
using Ozon.SeedClient.Models.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

SeedClient sd = new SeedClient();

await sd.SeedStorageActionsAsync(5);


// control by console

public class StorageActionKafkaMessage
{
    [JsonProperty]
    public Int32 storage_id { get; set; }

    [JsonProperty]
    public Int32 employee_id { get; set; }

    [JsonProperty]
    public Int32 action_id { get; set; }

    [JsonProperty]
    public string action_name { get; set; }

    [JsonProperty]
    public Int32 storage_cell_id { get; set; }

    [JsonProperty]
    public string storage_cell_name { get; set; }

    [JsonProperty]
    public Int32 storage_product_id { get; set; }

    [JsonProperty]
    public string external_product_id { get; set; }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }

    public StorageActionKafkaMessage(
        Int32 storageId,
        Int32 employeeId,
        Int32 actionId,
        string actionName,
        Int32 cellId,
        string cellName,
        Int32 storageProductId,
        string externalProductId)
    {
        Console.WriteLine("[ctor3]");
        external_product_id = externalProductId;
        storage_product_id = storageProductId;
        storage_cell_id = cellId;
        storage_cell_name = cellName;
        action_name = actionName;
        storage_id = storageId;
        employee_id = employeeId;
        action_id = actionId;
    }
}

class TestMessage
{
    [JsonProperty]
    public int Number { get; set; }

    [JsonProperty]
    public string Name { get; set; }

    public override string ToString()
    {
        return System.Text.Json.JsonSerializer.Serialize(this);
    }
}

public class StorageActionKafkaSerializer : ISerializer<StorageActionKafkaMessage>
{
    byte[] ISerializer<StorageActionKafkaMessage>.Serialize(StorageActionKafkaMessage data, SerializationContext context)
    {
        return Encoding.UTF8.GetBytes(data.ToString());
    }
}


class KafkaSerializer : ISerializer<StorageActionKafkaMessage>
{
    public byte[] Serialize(StorageActionKafkaMessage data, SerializationContext context)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (MemoryStream mem = new MemoryStream())
        {
            formatter.Serialize(mem, data);

            return mem.ToArray();
        }
    }
}

class OzonServiceApiClient
{
    private HttpClient _httpClient;

    private Logger<OzonServiceApiClient> _logger;

    public OzonServiceApiClient()
    {
        _httpClient = new HttpClient();


        var loggerFactory = LoggerFactory.Create(
                   builder => builder
                               // add console as logging target
                               .AddConsole()
                               // set minimum level to log
                               .SetMinimumLevel(LogLevel.Debug)
               );

        _logger = new Logger<OzonServiceApiClient>(loggerFactory);
    }

    public async Task<string[]> GetAllProducts()
    {
        try
        {
            var stringResponse = await _httpClient.GetAsync("http://ozon-api/products/all")
                .Result.Content
                .ReadAsStringAsync();

            GetAllProductsResponse dto = JsonConvert.DeserializeObject<GetAllProductsResponse>(stringResponse);

            return dto.Products.Select(x => x.Id)
                .ToArray();
        }
        catch (Exception exp)
        {
            _logger.LogError(exp.Message);

            return new string[0];
        }
    }

    public async Task<int[]> GetAllStorages()
    {
        try
        {
            var stringResponse = await _httpClient.GetAsync("http://ozon-api/storages/all")
                .Result.Content
                .ReadAsStringAsync();

            GetAllStoragesResponse dto = JsonConvert.DeserializeObject<GetAllStoragesResponse>(stringResponse);

            return dto.Storages.Select(x => x.StorageId)
                .ToArray();
        }
        catch (Exception exp)
        {
            _logger.LogError(exp.Message);

            return new int[0];
        }
    }

    public async Task<StorageActionTypeRead[]> GetAllActionTypes()
    {
        try
        {
            var getActionsStringResult = await _httpClient.GetAsync("http://ozon-api/Storages/actions/all")
                .Result.Content
                .ReadAsStringAsync();

            StorageActionsGetResponse getActions = JsonConvert.DeserializeObject<StorageActionsGetResponse>(getActionsStringResult);

            return getActions.Actions
                .ToArray();
        }
        catch (Exception exp)
        {
            _logger.LogError(exp.Message);

            return new StorageActionTypeRead[0];
        }
    }

    public async Task<int[]> GetStorageEmployees(uint specificStorageId)
    {
        try
        {
            var stringResponse = await _httpClient.GetAsync($"http://ozon-api/Storages/{specificStorageId}/employees")
                .Result.Content
                .ReadAsStringAsync();

            GetStorageEmployeesResponse dto = JsonConvert.DeserializeObject<GetStorageEmployeesResponse>(stringResponse);

            return dto.Employees.Select(x => x.StorageEmployeeId)
                .ToArray();
        }
        catch (Exception exp)
        {
            _logger.LogError(exp.Message);

            return new int[0];
        }
    }

    public async Task<IEnumerable<StorageProductRead>> GetStorageProducts(int storageId) 
    {
        try
        {
            var stringResponse = await _httpClient.GetAsync($"http://ozon-api/storages/{storageId}/products")
                .Result.Content
                .ReadAsStringAsync();

            GetStorageProductsResponse dto = JsonConvert.DeserializeObject<GetStorageProductsResponse>(stringResponse);

            return dto.Products;
        }
        catch (Exception exp)
        {
            _logger.LogError(exp.Message);

            return Enumerable.Empty<StorageProductRead>();
        }
    }

    public async Task<IEnumerable<StorageCellRead>> GetStorageCells(int storageId)
    {
        try
        {
            var stringResponse = await _httpClient.GetAsync($"http://ozon-api/storages/{storageId}/cells")
                .Result.Content
                .ReadAsStringAsync();

            GetAllStorageCellsResponse dto = JsonConvert.DeserializeObject<GetAllStorageCellsResponse>(stringResponse);

            return dto.Cells;
        }
        catch (Exception exp)
        {
            _logger.LogError(exp.Message);

            return Enumerable.Empty<StorageCellRead>();
        }
    }
}

class KafkaDataset
{
    private ProducerConfig config = new ProducerConfig()
    {
        BootstrapServers = "kafka-broker:9092",
        Acks = Acks.All,
        EnableBackgroundPoll = false,
    };

    private int[] _dataStoragesId;

    private StorageActionTypeRead[] _dataActions;

    private int[] _dataEmployeesId;

    private StorageProductRead[] _dataProducts;

    private StorageCellRead[] _dataCells;

    public KafkaDataset()
    {
        OzonServiceApiClient apiClient = new OzonServiceApiClient();

        Task.Run(async () =>
        {
            _dataActions = await apiClient.GetAllActionTypes();
            
            _dataEmployeesId = await apiClient.GetStorageEmployees(
                specificStorageId: 1);
            
            var prods = await apiClient.GetStorageProducts(1);
            _dataProducts = prods.ToArray();

            _dataStoragesId = await apiClient.GetAllStorages();

            var cells = await apiClient.GetStorageCells(1);
            _dataCells = cells.ToArray();
        })
        .Wait();
    }

    public void push_storage_actions(int iterationsCount)
    {
        Random rnd = new Random();

        ProducerBuilder<Int32, StorageActionKafkaMessage> builder = new ProducerBuilder<Int32, StorageActionKafkaMessage>(
           config.AsEnumerable());

        using (var prod = builder
            .SetValueSerializer(new StorageActionKafkaSerializer())
            .Build())
        {
            for (int i = 0; i < iterationsCount; i++)
            {
                StorageActionTypeRead action = _dataActions[rnd.Next(0, _dataActions.Length)];

                StorageCellRead cell = _dataCells[rnd.Next(0, _dataCells.Length)];

                StorageProductRead product = _dataProducts[rnd.Next(0, _dataProducts.Length)];

                StorageActionKafkaMessage messageValue = new StorageActionKafkaMessage(
                        storageId: _dataStoragesId[rnd.Next(0, _dataStoragesId.Length)],
                        employeeId: _dataEmployeesId[rnd.Next(0, _dataEmployeesId.Length)],
                        actionId: action.ActionId,
                        actionName: action.Name,
                        cellId: cell.CellId,
                        cellName: cell.CellNumber,
                        storageProductId: product.InStorageProductId,
                        externalProductId: product.ExternalProductId);

                prod.ProduceAsync("chance", new Message<Int32, StorageActionKafkaMessage>()
                {
                    Key = rnd.Next(0, 100),
                    Value = messageValue,
                });

                Console.WriteLine($"[st-actions] send message");
            }

            prod.Flush();
        }
    }

}

class SeedClient
{
    // seed kafka store-actions
    // fix click-house table?

    public Task SeedStorageActionsAsync(int taskc)
    {
        KafkaDataset kd = new KafkaDataset();

        Console.WriteLine("[push]");

        for (int i = 0; i < taskc; i++)
        {
            Task.Run(() =>
            {
                kd.push_storage_actions(5);
            })
                .Wait();
        }

        

        return Task.CompletedTask;
    }
}


//using var connection = new ClickHouseConnection("Host=click-house;Protocol=http;Port=2183;Username=user");
// ExecuteScalarAsync is an async extension which creates command and executes it

//var version = await connection.ExecuteScalarAsync("SELECT version()");
//Console.WriteLine(version);


//var config = new ProducerConfig
//{
//    BootstrapServers = "kafka-broker:9092",
//};

