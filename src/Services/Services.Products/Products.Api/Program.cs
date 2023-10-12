using Common.Repositories;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Products.Api.Extensions;
using Products.Data.Context;
using Products.Infrastructure.Repositories;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseMemoryStorage());

builder.Services.AddHangfireServer(options => {
    options.Queues = new[] { "consumers", "beta", "default" };
});

var connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddScoped(typeof(IServiceRepository<>), typeof(ServiceRepository<>));

builder.Services.AddConsumerFactory("kafka-broker:9092");
builder.Services.AddProducerFactory("kafka-broker:9092");

builder.Services.AddControllers();

builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => {
    x.AllowAnyOrigin();
    x.AllowAnyHeader();
});


//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
