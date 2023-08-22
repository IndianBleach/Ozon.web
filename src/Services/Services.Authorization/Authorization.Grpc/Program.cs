using Authorization.Grpc.Services;
using Authorization.Grpc.Services.Jwt;
using Authorization.Grpc.Services.Jwt.Client;
using Authorization.Grpc.Services.Jwt.Tokens;
using Authorization.Grpc.Services.RedisCache;
using Common.Config;
using Common.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// services
builder.Services.AddSingleton(new JwtOptionsProvider(
    issuer: JWTConfigOptions.ISSUER,
    audince: JWTConfigOptions.AUDINCE,
    key: JWTConfigOptions.SECRET_KEY,
    lifeTimeInMinutes: JWTConfigOptions.ACCESS_TOKEN_LIFETIME_MINS));

builder.Services.AddSingleton<IJwtTokenWorker, JwtTokenWorker>();

builder.Services.AddSingleton<IJwtClientService, JwtClientService>();

builder.Services.AddTransient<IAuthRedisService, AuthRedisService>();

builder.Services.AddGrpc();

builder.Services.AddGrpcReflection();

builder.Services.Scan(scan => scan.FromCallingAssembly()
        .AddClasses()
        .AsMatchingInterface());

builder.WebHost.ConfigureKestrel(option =>
{
    option.ListenAnyIP(6000, o => o.Protocols = HttpProtocols.Http2);
});

var app = builder.Build();

app.MapGrpcService<AuthorizationGrpc>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.MapGrpcReflectionService();

app.Run();
