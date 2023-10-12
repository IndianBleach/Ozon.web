namespace Storage.Api.ClickHouse
{
    internal class ClickHouseStorageServiceOptions
    {
        public string KafkaHost;

        public string KafkaPort;

        public string ProductMovemementsTopic;

        internal ClickHouseStorageServiceOptions(
            string kafkaHost,
            string kafkaPort,
            string productMovementTopic)
        {
            KafkaHost = kafkaHost;
            KafkaPort = kafkaPort;
            ProductMovemementsTopic = productMovementTopic;
        }
    }
}
