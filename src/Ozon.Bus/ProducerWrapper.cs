using Confluent.Kafka;
using Ozon.Bus.Serdes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus
{
    public class ProducerWrapper<TKey, TMessage> where TMessage : class
    {
        private IProducer<TKey, TMessage> _producer;

        public ProducerWrapper(IProducer<TKey, TMessage> producer)
        {
            _producer = producer;
        }

        public void PublishMessage(
            string toTopicAddr,
            Message<TKey, TMessage> message)
        {
            try
            {
                _producer.Produce(
                    topic: toTopicAddr,
                    message: message);

                Console.WriteLine("DELIVERY RESULT: " + message.Key);

                _producer.Flush();
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
                _producer.Produce(
                    topic: toTopicAddr,
                    message: message,
                    deliveryHandler: handler);

                _producer.Flush();
            }
            catch (Exception exp)
            {
                Console.WriteLine("PRODUCER ERROR: " + exp.Message);
            }
        }
    }
}
