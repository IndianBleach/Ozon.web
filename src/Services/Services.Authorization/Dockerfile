#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Services.Authorization/Authorization.Api/Authorization.Api.csproj", "Services/Services.Authorization/Authorization.Api/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "Services/Services.Authorization/Authorization.Api/Authorization.Api.csproj"
COPY . .
WORKDIR "/src/Services/Services.Authorization/Authorization.Api"
RUN dotnet build "Authorization.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Authorization.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authorization.Api.dll"]