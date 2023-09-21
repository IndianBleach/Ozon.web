using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ozon.Bus
{
    public class Consumer<TKey, TMessage> where TMessage : class
    {
        private bool _isEOF = false;

        private event EventHandler<TMessage> OnMessage;

        public event EventHandler<Exception> OnException;

        private readonly IConsumer<TKey, TMessage> _consumer;

        private void ExpHandler(object? sender, Exception exp)
        {
            Console.WriteLine(exp.Message);
        }

        public Consumer(
            ConsumerConfig config,
            IDeserializer<TMessage> valueDeserializator,
            EventHandler<Exception> onExeption)
        {
            OnException += onExeption;

            var builder = new ConsumerBuilder<TKey, TMessage>(config)
                .SetErrorHandler((_, error) => {
                    Console.WriteLine("[cons-error]");
                    if (_consumer != null)
                    {
                        _consumer.Commit();
                        _consumer.Close();
                    }
                    OnException(this, new Exception(error.Reason));
                })
                .SetValueDeserializer(valueDeserializator);

            _consumer = builder.Build();
        }

        public async void Start(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                
                try
                {
                    var consumeResult = _consumer.Consume();
                    Console.WriteLine("[executing consume...]");

                    if (consumeResult != null)
                    {
                        if (consumeResult.IsPartitionEOF)
                            _isEOF = true;

                        Console.WriteLine("[RECEIVE-MSG]");
                        OnMessage(this, consumeResult.Message.Value);
                    }
                    else
                    {
                        Console.WriteLine("EOF");
                        //_consumer.Commit();
                    }

                }
                catch (Exception exp)
                {
                    Console.WriteLine("[executing exception...] " + exp.Message);
                    //if (_consumer.)
                }
            }
        }

        public void Close()
        { 
            _consumer.Close();
        }

        public void Commit()
            => _consumer.Commit();

        public IObservable<TMessage> ConsumeObserv(IEnumerable<string> topics)
        {
            var obs = Observable.FromEventPattern<TMessage>(
                addHandler: (x) =>
                {
                    OnMessage += x;
                    _consumer.Subscribe(topics);
                },
                removeHandler: (x) =>
                {
                    _consumer.Unsubscribe();
                    OnMessage -= x;
                })
                .Select(x => x.EventArgs);

            return obs;
        }
    }
}
