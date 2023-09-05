using Common.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Storage.Data.Context;
using Storage.Grpc.ClickHouse;
using Storage.Grpc.Kafka;
using Storage.Grpc.Services;
using Storage.Infrastructure.Repositories;
using System.Net;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddHostedService<BackgroundConsumerService>(_ => new BackgroundConsumerService(
    kafkaHost: builder.Configuration["KafkaConfig:Hostname"],
    logger: _.GetRequiredService<ILogger<BackgroundConsumerService>>()));


Console.WriteLine("[After consumer]");

//TO CONFIG
builder.Services.AddSingleton(new ClickHouseStorageServiceProvider(
    hostName: builder.Configuration["ClickHouse:Hostname"],
    port: uint.Parse(builder.Configuration["ClickHouse:Port"]),
    userName: builder.Configuration["ClickHouse:Username"],
    kafkaHost: builder.Configuration["ClickHouse:KafkaHost"]));

Console.WriteLine("[After house]");

var connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

if (string.IsNullOrEmpty(connection))
    builder.Services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("TESTING"));
else
    builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddScoped(typeof(IServiceRepository<>), typeof(ServiceRepository<>));

builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();

builder.WebHost.ConfigureKestrel(option =>
{
    option.Listen(IPAddress.Any, 5010, x => x.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();


app.MapGrpcService<StorageService>();

app.MapGrpcReflectionService();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
