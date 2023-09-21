using Common.Repositories;
using Hangfire;
using Hangfire.MemoryStorage;
using Marketplace.Api.Kafka.Producers;
using Marketplace.Data.Context;
using Marketplace.Infrastructure.BusServices;
using Marketplace.Infrastructure.Repositories;
using Marketplace.Infrastructure.Repositories.Products;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddTransient<IProductRegistryProducer, ProductRegistryProducer>();

// elastic, redis
//builder.Services.AddGrpc();

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
