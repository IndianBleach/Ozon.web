namespace Storage.Api.Kafka
{
    public class KafkaOptions
    {
        public string Hostname { get; set; }

        public KafkaOptions(string host)
        {
            Hostname = host;
        }
    }
}
