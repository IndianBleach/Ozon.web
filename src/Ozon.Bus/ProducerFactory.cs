using Confluent.Kafka;
using Ozon.Bus.Serdes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Ozon.Bus
{
    public class ProducerFactory : IProducerFactory
    {
        Dictionary<string, ProducerConfig> _producers = new Dictionary<string, ProducerConfig>();

        private string _getProducerName<TKey, TMessage>()
            => $"{typeof(TKey)}.{typeof(TMessage)}";

        public ProducerWrapper<TKey, TMessage>? Get<TKey, TMessage>() where TMessage : TMessageBusValue
        {
            ProducerConfig? findConfig;
            _producers.TryGetValue(
                key: _getProducerName<TKey, TMessage>(),
                out findConfig);

            if (findConfig == null)
                return null;

            var builder = new ProducerBuilder<TKey, TMessage>(
                findConfig.AsEnumerable())
                .SetValueSerializer(new ServiceBusValueSerializer<TMessage>())
                .SetErrorHandler((_, error) => {
                    Console.WriteLine("PRODUCER ERROR: " + error.Reason);
                });

            return new ProducerWrapper<TKey, TMessage>(builder.Build());
        }

        public void Register<TKey, TMessage>(ProducerConfig config) where TMessage : TMessageBusValue
        {
            string producerName = _getProducerName<TKey, TMessage>();

            if (!_producers.ContainsKey(producerName))
            {
                _producers.Add(
                    key: producerName,
                    value: config);
            }
        }
    }
}
