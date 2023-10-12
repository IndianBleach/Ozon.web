using Common.Repositories;
using Confluent.Kafka;
using Hangfire;
using Hangfire.MemoryStorage;
using Marketplace.Api.Extensions;
using Marketplace.Api.Kafka;
using Marketplace.Data.Context;
using Marketplace.Infrastructure.BusServices;
using Marketplace.Infrastructure.Repositories;
using Marketplace.Infrastructure.Repositories.Products;
using Microsoft.EntityFrameworkCore;
using Ozon.Bus;
using Ozon.Bus.DTOs.ProductsRegistry;
using Ozon.Bus.DTOs.StorageService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMemoryStorage());

builder.Services.AddHangfireServer(options => {
    options.Queues = new[] { "consumers", "beta", "default" };
});

var connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection), ServiceLifetime.Transient);

builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddTransient(typeof(IServiceRepository<>), typeof(ServiceRepository<>));

builder.Services.AddTransient<IMarketplaceProductBusService, MarketplaceProductBusService>();

string kafkaHost = "kafka-broker:9092";
builder.Services.AddConsumerFactory(kafkaHost);
builder.Services.AddProducerFactory(kafkaHost);

// elastic, redis

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHangfireDashboard();

app.UseCors(builder => {
    builder.AllowAnyOrigin();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
});

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
