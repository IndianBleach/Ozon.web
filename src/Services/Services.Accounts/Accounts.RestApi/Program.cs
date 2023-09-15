using Accounts.Infrastructure.HelpServices;
using Accounts.Infrastructure.Repositories;
using Common.Repositories;
using Microsoft.EntityFrameworkCore;
using User.Data.Context;

var builder = WebApplication.CreateBuilder(args);


string? connection = builder.Configuration.GetConnectionString("MssqlConnectionString");

Console.WriteLine("CONNECTION STRING " + connection);

if (string.IsNullOrEmpty(connection))
{
    builder.Services.AddDbContext<ApplicationContext>(options => options.UseInMemoryDatabase("TESTING"));
}
else
{
    builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
}

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new JwtHelpService(
    key: builder.Configuration["PasswordHasher:jwtKey"],
    issuer: builder.Configuration["PasswordHasher:jwtIssuer"],
    audince: builder.Configuration["PasswordHasher:jwtAudince"]));

builder.Services.AddSingleton(new PasswordHashHelpService(
    hashSalt: builder.Configuration["PasswordHasher:hashSalt"]));

builder.Services.AddScoped(typeof(IServiceRepository<>), typeof(ServiceRepository<>));

builder.Services.AddTransient<IUserAccumulatorService, UserAccumulatorService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
