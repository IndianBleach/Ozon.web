using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus
{
    public class ConsumerWrapper<TKey, TMessage> : IMessageBusConsumer<TMessage>
    {
        private event EventHandler<TMessage> _onMessage;

        private readonly IConsumer<TKey, TMessage> _consumer;

        public ConsumerWrapper(IConsumer<TKey, TMessage> consumer)
        {
            _consumer = consumer;
        }

        public void StartConsume(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume();
                    Console.WriteLine("[executing consume...]");

                    if (consumeResult != null)
                    {
                        Console.WriteLine("[RECEIVE-MSG]");
                        if (_onMessage != null)
                            _onMessage(this, consumeResult.Message.Value);
                    }
                }
                catch (Exception exp)
                {
                    Console.WriteLine("[executing exception...] " + exp.Message);
                }
            }
        }

        public void Close()
            => _consumer.Close();

        public void Commit()
            => _consumer.Commit();

        public IObservable<TMessage> ObservableData(IEnumerable<string> topics)
        {
            var obs = Observable.FromEventPattern<TMessage>(
                addHandler: (x) =>
                {
                    _onMessage += x;
                    _consumer?.Subscribe(topics);
                },
                removeHandler: (x) =>
                {
                    _consumer?.Unsubscribe();
                    _onMessage -= x;
                })
                .Select(x => x.EventArgs);

            return obs;
        }
    }
}
