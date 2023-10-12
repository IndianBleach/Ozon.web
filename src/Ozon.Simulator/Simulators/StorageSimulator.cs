using Common.DTOs.Catalog;
using Common.DTOs.Storage;
using Confluent.Kafka;
using Ozon.Bus;
using Ozon.Simulator.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Simulator.Simulators
{
    public class StorageActionKafkaSerializer : ISerializer<StorageProductMovementCreate>
    {
        byte[] ISerializer<StorageProductMovementCreate>.Serialize(StorageProductMovementCreate data, SerializationContext context)
        {
            return Encoding.UTF8.GetBytes(data.ToString());
        }
    }

    internal enum Simulator_ProductMovementsActions
    { 
        PutToCell,
        GetFromCell,
    }

    internal class StorageSimulator
    {
        private ApiClient _api = new ApiClient();

        private Random _rnd = new Random();

        public StorageSimulator()
        {
        }

        public async Task SimulateProductsMovements(int iterationsCount, Simulator_ProductMovementsActions actionType)
        {
            StorageRead[] _storages = await _api.GetDataAsync<StorageRead[]>("http://ozon-api/storages");

            int randomStorageId = _storages[_rnd.Next(0, _storages.Length)].Id;

            StorageActionRead[] _storageActions = await _api.GetDataAsync<StorageActionRead[]>("http://ozon-api/storages/actions");
            StorageProductRead[] _storageProducts = await _api.GetDataAsync<StorageProductRead[]>($"http://ozon-api/storages/{randomStorageId}/products");
            StorageCellRead[] _storageCells = await _api.GetDataAsync<StorageCellRead[]>($"http://ozon-api/storages/{randomStorageId}/cells");

            Console.WriteLine($"[SimulateProductsMovements] api-data: {_storages.Length}, {_storageActions.Length}, {_storageProducts.Length}, {_storageCells.Length}");

            var config = new ProducerConfig()
            {
                BootstrapServers = "kafka-broker:9092",
                Acks = Acks.All,
                EnableBackgroundPoll = false
            };

            ProducerBuilder <Int32, StorageProductMovementCreate> builder = new ProducerBuilder<Int32, StorageProductMovementCreate>(
               config.AsEnumerable());

            var singleProducer = builder
                .SetValueSerializer(new StorageActionKafkaSerializer())
                .Build();

            StorageActionRead action = _storageActions.First(x => x.Id == 1);
            Console.WriteLine("[action type name] " + action.Name);

            while (iterationsCount-- > 0)
            {
                
                StorageCellRead cell = _storageCells[_rnd.Next(0, _storageCells.Length)];
                StorageProductRead product = _storageProducts[_rnd.Next(0, _storageProducts.Length)];

                // q? can i produce batch to clickhouse?

                singleProducer.Produce(
                    topic: "clickhouse_channel-productMovements",
                    message: new Message<Int32, StorageProductMovementCreate>
                    {
                        Key = _rnd.Next(Int32.MinValue, Int32.MaxValue),
                        Value = new StorageProductMovementCreate(
                            storageId: randomStorageId,
                            actionId: action.Id,
                            actionName: action.Name,
                            cellId: cell.Id,
                            cellName: cell.Name,
                            storageProductId: product.StorageProductId,
                            marketplaceProductId: product.MarketplaceProductId)
                    },
                    deliveryHandler: (report) =>
                    {
                        Console.WriteLine("[message sended] " + report.Status);
                    });

                singleProducer.Flush();
            }
        }
    }
}
