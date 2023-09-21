using Confluent.Kafka;
using Ozon.Bus.Serdes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus
{
    public class Producer<TKey, TMessage> where TMessage : class
    {
        private ProducerConfig _producerConfig;

        private ProducerBuilder<TKey, TMessage> _builder;

        public Producer(ProducerConfig config)
        {
            _producerConfig = config;

            _builder = new ProducerBuilder<TKey, TMessage>(
                _producerConfig.AsEnumerable())
                .SetValueSerializer(new ServiceBusValueSerializer<TMessage>())
                .SetErrorHandler((_, error) => {
                    Console.WriteLine("PRODUCER ERROR: " + error.Reason);
                });
        }

        public void PublishMessage(
            string toTopicAddr,
            Message<TKey, TMessage> message)
        {
            try
            {
                using var producer = _builder
                .Build();

                producer.Produce(
                    topic: toTopicAddr,
                    message: message);

                Console.WriteLine("DELIVERY RESULT: " + message.Key);

                producer.Flush();
            }
            catch (Exception exp)
            {
                Console.WriteLine("PRODUCER ERROR: " + exp.Message);
            }
        }

        public void PublishMessage(
            string toTopicAddr,
            Message<TKey, TMessage> message,
            Action<DeliveryReport<TKey, TMessage>> handler)
        {
            try
            {
                using var producer = _builder
                .Build();

                producer.Produce(
                    topic: toTopicAddr,
                    message: message,
                    deliveryHandler: handler);

                producer.Flush();
            }
            catch (Exception exp)
            {
                Console.WriteLine("PRODUCER ERROR: " + exp.Message);
            }
        }
    }
}
