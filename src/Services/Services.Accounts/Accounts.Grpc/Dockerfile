#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Services.Accounts/Accounts.Grpc/Accounts.Grpc.csproj", "Services/Services.Accounts/Accounts.Grpc/"]
COPY ["Services/Services.Accounts/Accounts.Data/Accounts.Data.csproj", "Services/Services.Accounts/Accounts.Data/"]
COPY ["Services/Services.Accounts/Accounts.Infrastructure/Accounts.Infrastructure.csproj", "Services/Services.Accounts/Accounts.Infrastructure/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "Services/Services.Accounts/Accounts.Grpc/Accounts.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/Services.Accounts/Accounts.Grpc"
RUN dotnet build "Accounts.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Accounts.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Accounts.Grpc.dll"]