using Authorization.Grpc.Services;
using Authorization.Grpc.Services.Jwt;
using Authorization.Grpc.Services.Jwt.Client;
using Authorization.Grpc.Services.Jwt.Tokens;
using Authorization.Grpc.Services.RedisCache;

var builder = WebApplication.CreateBuilder(args);

// services
builder.Services.AddSingleton(new JwtOptionsProvider(
    issuer: builder.Configuration["JwtAuth:Issuer"],
    audince: builder.Configuration["JwtAuth:Audince"],
    key: builder.Configuration["JwtAuth:SecretKey"],
    lifeTimeInMinutes: int.Parse(builder.Configuration["JwtAuth:AccessTokenLifeTime"])));

builder.Services.AddSingleton<IJwtTokenWorker, JwtTokenWorker>();

builder.Services.AddSingleton<IJwtClientService, JwtClientService>();

builder.Services.AddTransient<IAuthRedisService, AuthRedisService>();

builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<AuthorizationGrpc>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
