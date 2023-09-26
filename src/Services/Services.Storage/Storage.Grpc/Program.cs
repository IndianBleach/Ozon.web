using Common.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Storage.Data.Context;
using Storage.Grpc.ClickHouse;
using Storage.Grpc.Extensions;
using Storage.Grpc.Services;
using Storage.Infrastructure.Repositories;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();

Console.WriteLine("[After consumer]");

//TO CONFIG
//builder.Services.AddSingleton(new ClickHouseStorageServiceProvider(
//    hostName: builder.Configuration["ClickHouse:Hostname"],
//    port: uint.Parse(builder.Configuration["ClickHouse:Port"]),
//    userName: builder.Configuration["ClickHouse:Username"],
//    kafkaHost: builder.Configuration["ClickHouse:KafkaHost"]));

Console.WriteLine("[After house]");


builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();

builder.Services.AddProducerFactory("kafka-broker:9092");


var app = builder.Build();

app.UseRouting();

app.MapGrpcService<StorageService>();

app.MapGrpcReflectionService();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
