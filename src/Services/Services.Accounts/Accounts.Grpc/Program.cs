using Accounts.Data.Entities.AccountEntities;
using Accounts.Grpc.GrpcServices;
using Accounts.Infrastructure.Repositories;
using Common.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;
using User.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// add context setup docker
// 

//.builder.Configuration.GetConnectionString("MssqlConnectionString");
string? connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

builder.Services.AddScoped(typeof(IServiceRepository<>), typeof(ServiceRepository<>));

Console.WriteLine("CONNECTION STRING " + connection);

if (string.IsNullOrEmpty(connection))
{
    builder.Services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("TESTING"));
}
else
{
    builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
}


builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();

builder.WebHost.ConfigureKestrel(option =>
{
    option.Listen(IPAddress.Any, 5005, x => x.Protocols = HttpProtocols.Http2);
});


var app = builder.Build();

app.MapGrpcService<UserAccountGrpc>();
app.MapGrpcService<AccountSecurityService>();

app.MapGrpcReflectionService();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
