using Common.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Products.Data.Context;
using Products.Grpc.Services;
using Products.Infrastructure.Repositories;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var connection = "";

if (string.IsNullOrEmpty(connection))
{
    builder.Services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("TESTING"));
}
else
{
    builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
}

builder.Services.AddScoped(typeof(IServiceRepository<>), typeof(ServiceRepository<>));

builder.Services.AddGrpc();

//builder.Services.AddGrpcReflection();

builder.WebHost.ConfigureKestrel(option =>
{
    option.Listen(IPAddress.Any, 5006, x => x.Protocols = HttpProtocols.Http2);
});


var app = builder.Build();


app.MapGrpcService<ProductService>();

//app.MapGrpcReflectionService();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

