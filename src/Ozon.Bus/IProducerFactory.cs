using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus
{
    public interface IProducerFactory
    {
        ProducerWrapper<TKey, TMessage>? Get<TKey, TMessage>()
            where TMessage : TMessageBusValue;

        ProducerWrapper<TKey, List<TMessage>>? GetBatch<TKey, TMessage>()
            where TMessage : TMessageBusValue;

        void Register<TKey, TMessage>(ProducerConfig config)
            where TMessage : TMessageBusValue;
    }
}
