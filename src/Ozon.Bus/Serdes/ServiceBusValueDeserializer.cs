using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ozon.Bus.Serdes
{
    public class ServiceBusValueDeserializer<T> : IDeserializer<T> where T : class
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var bytesAsString = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(bytesAsString);
        }
    }

    public class ServiceBusValueSerializer<T> : ISerializer<T> where T : class 
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            return Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(data));
        }
    }
}
