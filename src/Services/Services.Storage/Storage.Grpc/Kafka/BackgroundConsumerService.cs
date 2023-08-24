using Confluent.Kafka;

namespace Storage.Grpc.Kafka
{
    public class BackgroundConsumerService : BackgroundService
    {
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "foo",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe("test-topic");

                while (!stoppingToken.IsCancellationRequested)
                {
                    ConsumeResult<Ignore, string> consumeResult = consumer.Consume(stoppingToken);

                    Console.WriteLine("Tick: " + consumeResult.Value);
                }
            }
            

            return Task.CompletedTask;
        }
    }
}
