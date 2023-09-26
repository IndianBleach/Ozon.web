using Confluent.Kafka;
using Ozon.Bus.Serdes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Confluent.Kafka.ConfigPropertyNames;

namespace Ozon.Bus
{
    // register
    // get
    // update

    public abstract class TMessageBusValue
    { }

    public abstract class TMessageBusKey
    { }

    public class ConsumerFactory : IConsumerFactory
    {
        Dictionary<string, ConsumerConfig> _consumers = new Dictionary<string, ConsumerConfig>();

        private string _getConsumerName<TKey, TMessage>()
            => $"{typeof(TKey)}.{typeof(TMessage)}";

        public ConsumerWrapper<TKey, TMessage>? GetSingle<TKey, TMessage>()
            where TMessage : TMessageBusValue
        {
            ConsumerConfig? findConfig;

            bool findResult = _consumers.TryGetValue(
                key: _getConsumerName<TKey, TMessage>(),
                out findConfig);

            if (!findResult)
                return null;

            var builder = new ConsumerBuilder<TKey, TMessage>(findConfig)
                .SetErrorHandler((consumer, error) => {
                    Console.WriteLine("[cons-error]");
                    if (consumer != null)
                    {
                        try
                        {
                            consumer.Commit();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("commit error: " + ex.Message);
                        }
                        finally
                        {
                            consumer.Close();
                        }
                    }
                    //onExeption(this, new Exception(error.Reason));
                })
                .SetValueDeserializer(new ServiceBusValueDeserializer<TMessage>());

            return new ConsumerWrapper<TKey, TMessage>(builder.Build());
        }

        public ConsumerWrapper<TKey, ReadOnlyCollection<TMessage>>? GetBatch<TKey, TMessage>()
           where TMessage : TMessageBusValue
        {
            ConsumerConfig? findConfig;

            bool findResult = _consumers.TryGetValue(
                key: _getConsumerName<TKey, TMessage>(),
                out findConfig);

            if (!findResult)
                return null;

            var builder = new ConsumerBuilder<TKey, ReadOnlyCollection<TMessage>>(findConfig)
                .SetErrorHandler((consumer, error) => {
                    Console.WriteLine("[cons-error]");
                    if (consumer != null)
                    {
                        try
                        {
                            consumer.Commit();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("commit error: " + ex.Message);
                        }
                        finally
                        {
                            consumer.Close();
                        }
                    }
                    //onExeption(this, new Exception(error.Reason));
                })
                .SetValueDeserializer(new ServiceBusValueDeserializer<ReadOnlyCollection<TMessage>>());

            return new ConsumerWrapper<TKey, ReadOnlyCollection<TMessage>>(builder.Build());
        }

        public void Register<TKey, TMessage>(ConsumerConfig config)
            where TMessage : TMessageBusValue
        {
            string consumerName = _getConsumerName<TKey, TMessage>();

            if (!_consumers.ContainsKey(consumerName))
            {
                _consumers.Add(
                    key: consumerName,
                    value: config);
            }
        }
    }
}
