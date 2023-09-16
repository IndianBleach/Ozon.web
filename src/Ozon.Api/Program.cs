using Common.Config;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.AddControllers();


// REVERSE PROXY
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddHttpClient("Accounts", client =>
{
    client.BaseAddress = new Uri("http://ozon-api/accounts/");
});

builder.Services.AddHttpClient("Storages", client =>
{
    client.BaseAddress = new Uri("http://ozon-api/storages/");
});

builder.Services.AddHttpClient("Authorize", client =>
{
    client.BaseAddress = new Uri("http://ozon-api/authorize/");
});

builder.Services.AddHttpClient("Products", client =>
{
    client.BaseAddress = new Uri("http://ozon-api/products/");
});

builder.Services.AddHttpClient("Catalogs", client =>
{
    client.BaseAddress = new Uri("http://ozon-api/catalogs/");
});
///////////


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        //opt.Audience = JWTConfigOptions.AUDINCE;
        opt.RequireHttpsMetadata = false;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = JWTConfigOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = JWTConfigOptions.AUDINCE,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JWTConfigOptions.SECRET_KEY))
        };
    });

var app = builder.Build();

app.UseCors(builder => {
    builder.AllowAnyOrigin();
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
});

Console.WriteLine("ACCESS ANY ORIGIN (CORS)");

app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy();
});

app.Run();
