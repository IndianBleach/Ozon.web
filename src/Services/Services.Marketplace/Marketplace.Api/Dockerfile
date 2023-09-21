#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Services.Marketplace/Marketplace.Api/Marketplace.Api.csproj", "Services/Services.Marketplace/Marketplace.Api/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/Services.Marketplace/Marketplace.Data/Marketplace.Data.csproj", "Services/Services.Marketplace/Marketplace.Data/"]
COPY ["Services/Services.Marketplace/Marketplace.Infrastructure/Marketplace.Infrastructure.csproj", "Services/Services.Marketplace/Marketplace.Infrastructure/"]
RUN dotnet restore "Services/Services.Marketplace/Marketplace.Api/Marketplace.Api.csproj"
COPY . .
WORKDIR "/src/Services/Services.Marketplace/Marketplace.Api"
RUN dotnet build "Marketplace.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Marketplace.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Marketplace.Api.dll"]