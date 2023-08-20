using Accounts.Data.Entities.AccountEntities;
using Accounts.Grpc.GrpcServices;
using Common.Repositories;
using Microsoft.EntityFrameworkCore;
using User.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// add context setup docker
// 

string? connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

builder.Services.Scan(scan => scan
  .FromTypes(typeof(IServiceRepository<>))
    .AsSelf()
    .WithTransientLifetime());


Console.WriteLine("CONNECTION STRING " + connection);

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddGrpc();

var app = builder.Build();


//app.MapGrpcService<UserAccountGrpc>();


app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
