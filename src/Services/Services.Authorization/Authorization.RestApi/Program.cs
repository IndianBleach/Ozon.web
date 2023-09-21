using Authorization.RestApi.Services.Jwt;
using Authorization.RestApi.Services.Jwt.Client;
using Authorization.RestApi.Services.Jwt.Tokens;
using Authorization.RestApi.Services.RedisCache;
using Common.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton(new JwtAccountsOptionsProvider(
    validAudince: JWTAccountsConfigOptions.AUDINCE,
    validIssuer: JWTAccountsConfigOptions.ISSUER,
    validSecretKey: JWTAccountsConfigOptions.SECRET_KEY));

builder.Services.AddSingleton(new JwtOptionsProvider(
    issuer: JWTConfigOptions.ISSUER,
    audince: JWTConfigOptions.AUDINCE,
    key: JWTConfigOptions.SECRET_KEY,
    lifeTimeInMinutes: JWTConfigOptions.ACCESS_TOKEN_LIFETIME_MINS));

builder.Services.AddTransient<IJwtTokenWorker, JwtTokenWorker>();
builder.Services.AddTransient<IAuthRedisService, AuthRedisService>();
builder.Services.AddTransient<IJwtClientService, JwtClientService>();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseCors(builder => {
    builder.AllowAnyOrigin();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
