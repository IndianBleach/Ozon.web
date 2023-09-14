using Common.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Products.Data.Context;
using Products.Infrastructure.Repositories;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("test"));

builder.Services.AddScoped(typeof(IServiceRepository<>), typeof(ServiceRepository<>));

builder.Services.AddControllers();

builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddGrpc();

//builder.WebHost.ConfigureKestrel(option =>
//{
//    option.Listen(, 5000, x => x.Protocols = HttpProtocols.Http1);
//    option.Listen(IPAddress.Any, 5001, x => x.Protocols = HttpProtocols.Http2);
//});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => {
    x.AllowAnyOrigin();
    x.AllowAnyHeader();
});

//app.MapGrpcService<ProductService>();

//app.MapGrpcReflectionService();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
