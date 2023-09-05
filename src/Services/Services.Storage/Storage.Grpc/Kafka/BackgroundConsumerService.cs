using Common.Kafka;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace Storage.Grpc.Kafka
{
    class StorageActionDeserializer : IDeserializer<StorageActionKafkaRead>
    {
        public StorageActionKafkaRead Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            BinaryFormatter fm = new BinaryFormatter();

            try
            {
                return (StorageActionKafkaRead)fm.Deserialize(new MemoryStream(
                    buffer: data.ToArray()));
            }
            catch (Exception exp)
            {
                Console.WriteLine("DESERIALIZE ERROR: " + exp.Message);
                return null;
            }
        }
    }

    public class BackgroundConsumerService : BackgroundService
    {
        private readonly string _kafkaServer;

        private readonly ILogger<BackgroundConsumerService> _logger;

        public BackgroundConsumerService(
            string kafkaHost,
            ILogger<BackgroundConsumerService> logger)
        {
            _logger = logger;
            _kafkaServer = kafkaHost;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var cf = new ConsumerConfig()
            {
                GroupId = "storage-srv",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                BootstrapServers = _kafkaServer,
                EnableAutoCommit = false,
                AutoCommitIntervalMs = 0,
            };

            ConsumerBuilder<string, string> builder = new ConsumerBuilder<string, string>(cf);

            Console.WriteLine("[start-consumer]");

            try
            {
                using (var consumer = builder
                //.SetValueDeserializer(new StorageActionDeserializer())
                .Build())
                {
                    consumer.Subscribe("storage_actions");

                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var result = consumer.Consume(
                            timeout: TimeSpan.FromSeconds(5));

                        if (result == null)
                            throw new Exception("result is null");

                        if (result != null && !result.IsPartitionEOF)
                        {
                            StorageActionKafkaRead? data = JsonConvert.DeserializeObject<StorageActionKafkaRead>(result.Message.Value);

                            if (data != null)
                            {
                                Console.WriteLine("[storage_action]: " + $"{data.action_name}, {data.storage_product_id}");
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            { 
                _logger.LogError(exp.Message);
            }

            return Task.CompletedTask;
        }
    }
}
