#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Services.Accounts/Accounts.RestApi/Accounts.RestApi.csproj", "Services/Services.Accounts/Accounts.RestApi/"]
COPY ["Services/Services.Accounts/Accounts.Data/Accounts.Data.csproj", "Services/Services.Accounts/Accounts.Data/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/Services.Accounts/Accounts.Infrastructure/Accounts.Infrastructure.csproj", "Services/Services.Accounts/Accounts.Infrastructure/"]
RUN dotnet restore "Services/Services.Accounts/Accounts.RestApi/Accounts.RestApi.csproj"
COPY . .
WORKDIR "/src/Services/Services.Accounts/Accounts.RestApi"
RUN dotnet build "Accounts.RestApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Accounts.RestApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Accounts.RestApi.dll"]