using Authorization.Api.Services.Jwt;
using Authorization.Api.Services.Jwt.Client;
using Authorization.Api.Services.Jwt.Tokens;
using Authorization.Api.Services.RedisCache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// services
builder.Services.AddSingleton(new JwtOptionsProvider(
    issuer: builder.Configuration["JwtAuth:Issuer"],
    audince: builder.Configuration["JwtAuth:Audince"],
    key: builder.Configuration["JwtAuth:SecretKey"],
    lifeTimeInMinutes: int.Parse(builder.Configuration["JwtAuth:AccessTokenLifeTime"])));

builder.Services.AddSingleton<IJwtTokenWorker, JwtTokenWorker>();

builder.Services.AddSingleton<IJwtClientService, JwtClientService>();

builder.Services.AddTransient<IAuthRedisService, AuthRedisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
