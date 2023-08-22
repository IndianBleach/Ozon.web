#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5001
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Services/Services.Authorization/Authorization.Grpc/Authorization.Grpc.csproj", "Services/Services.Authorization/Authorization.Grpc/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "Services/Services.Authorization/Authorization.Grpc/Authorization.Grpc.csproj"
COPY . .
WORKDIR "/src/Services/Services.Authorization/Authorization.Grpc"
RUN dotnet build "Authorization.Grpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Authorization.Grpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Authorization.Grpc.dll"]