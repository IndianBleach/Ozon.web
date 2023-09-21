#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Services.Storage/Storage.Grpc/Storage.Grpc.csproj", "Services/Services.Storage/Storage.Grpc/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Ozon.Bus/Ozon.Bus.csproj", "Ozon.Bus/"]
COPY ["Services/Services.Storage/Storage.Data/Storage.Data.csproj", "Services/Services.Storage/Storage.Data/"]
COPY ["Services/Services.Storage/Storage.Infrastructure/Storage.Infrastructure.csproj", "Services/Services.Storage/Storage.Infrastructure/"]
RUN dotnet restore "Services/Services.Storage/Storage.Grpc/Storage.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/Services.Storage/Storage.Grpc"
RUN dotnet build "Storage.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Storage.Grpc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Storage.Grpc.dll"]