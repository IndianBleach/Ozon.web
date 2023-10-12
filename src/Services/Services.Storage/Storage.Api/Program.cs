using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Common.Repositories;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Storage.Api.ClickHouse;
using Storage.Api.Extensions;
using Storage.Api.Kafka;
using Storage.Data.Context;
using Storage.Data.Entities.Products;
using Storage.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// сколько перемещений
// перемещений в минуту

// сколько подключений к бд
// сколько ошибок при работе с кафкой
// сколько обращений к кешу

// kafka - сколько товаров пришло в хранилище +

var connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

builder.Host.UseMetricsWebTracking();
builder.Host.UseMetrics(options => {
    options.EndpointOptions = (endpointOpt) => {
        endpointOpt.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
        endpointOpt.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
        endpointOpt.EnvironmentInfoEndpointEnabled = true;
    };
});

builder.Services.AddMetrics();



builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMemoryStorage());

builder.Services.AddConsumerFactory("kafka-broker:9092");
builder.Services.AddProducerFactory("kafka-broker:9092");

builder.Services.AddHangfireServer(options => {
    options.Queues = new[] { "consumers", "beta", "default" };
});

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(connection);
});

builder.Services.AddScoped(typeof(IServiceRepository<>), typeof(ServiceRepository<>));

builder.Services.AddSingleton(new KafkaOptions(
    host: "kafka-broker:9092"));

builder.Services.AddClickHouseStorageClient(
    ch_connectionString: builder.Configuration.GetConnectionString("ClickHouseStorageDb"),
    chOptions: new ClickHouseStorageServiceOptions(
        kafkaHost: builder.Configuration["ClickHouse_kafka:KafkaHost"],
        kafkaPort: builder.Configuration["ClickHouse_kafka:KafkaPort"],
        productMovementTopic: builder.Configuration["ClickHouse_kafka:ProductMovementTopic"]));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHangfireDashboard();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
