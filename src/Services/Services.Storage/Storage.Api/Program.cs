using Common.Repositories;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Storage.Api.Extensions;
using Storage.Api.Kafka;
using Storage.Data.Context;
using Storage.Data.Entities.Products;
using Storage.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


var connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

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


builder.Services.Configure<HostOptions>(config =>
{
    config.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

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
