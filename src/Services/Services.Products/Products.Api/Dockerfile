#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Services.Products/Products.Api/Products.Api.csproj", "Services/Services.Products/Products.Api/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/Services.Products/Products.Data/Products.Data.csproj", "Services/Services.Products/Products.Data/"]
COPY ["Services/Services.Products/Products.Infrastructure/Products.Infrastructure.csproj", "Services/Services.Products/Products.Infrastructure/"]
RUN dotnet restore "Services/Services.Products/Products.Api/Products.Api.csproj"
COPY . .
WORKDIR "/src/Services/Services.Products/Products.Api"
RUN dotnet build "Products.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Products.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Products.Api.dll"]