using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus
{
    public interface IMessageBusConsumer<TMessage>
    {
        void Close();

        void StartConsume(CancellationToken token);

        IObservable<TMessage> ObservableData(IEnumerable<string> topics);
    }

    public interface IConsumerFactory
    {
        void Register<TKey, TMessage>(
            ConsumerConfig config)
            where TMessage : TMessageBusValue;

        ConsumerWrapper<TKey, TMessage>? GetSingle<TKey, TMessage>()
            where TMessage : TMessageBusValue;

        ConsumerWrapper<TKey, ReadOnlyCollection<TMessage>>? GetBatch<TKey, TMessage>()
            where TMessage : TMessageBusValue;
    }
}
