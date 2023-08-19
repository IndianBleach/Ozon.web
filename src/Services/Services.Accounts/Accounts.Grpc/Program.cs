using Accounts.Grpc.GrpcServices;
using Infrastructure.DataServices;
using Microsoft.EntityFrameworkCore;
using User.Data.Context;
using User.Infrastructure.DataServices;

var builder = WebApplication.CreateBuilder(args);

// add context setup docker
// 

string connection = string.Empty;

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

builder.Services.AddTransient<IUserAccountService, UserAccountService>();

builder.Services.AddGrpc();

var app = builder.Build();


app.MapGrpcService<UserAccountGrpc>();




app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
