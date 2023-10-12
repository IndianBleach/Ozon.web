using App.Metrics.Counter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Storage.Infrastructure.Metrics
{
    public class StorageMetricsRegistry
    {
        public static CounterOptions Kafka_productRegistrationCounter => new CounterOptions
        {
            Name = "(kafka) Product registration",
            Context = "storageRegistry-api",
            MeasurementUnit = App.Metrics.Unit.Calls,
            Tags = new App.Metrics.MetricTags("type", "kafka")
        };

        public static CounterOptions gRPC_productRegistrationCounter => new CounterOptions
        {
            Name = "(gRPC) Product movement",
            Context = "storageRegistry-api",
            MeasurementUnit = App.Metrics.Unit.Calls,
            Tags = new App.Metrics.MetricTags("type", "gRPC")
        };

        public static CounterOptions Kafka_productMovementCounter => new CounterOptions
        {
            Name = "(kafka+clickhouse) Product movement",
            Context = "storageRegistry-api",
            MeasurementUnit = App.Metrics.Unit.Calls,
            Tags = new App.Metrics.MetricTags("type", "kafka")
        };

    }
}
