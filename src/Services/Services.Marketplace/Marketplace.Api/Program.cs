using Common.Repositories;
using Marketplace.Data.Context;
using Marketplace.Infrastructure.Repositories;
using Marketplace.Infrastructure.Repositories.Products;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddScoped(typeof(IServiceRepository<>), typeof(ServiceRepository<>));

// elastic, redis
//builder.Services.AddGrpc();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors(x =>
{
    x.AllowAnyOrigin();
    x.AllowAnyHeader();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
