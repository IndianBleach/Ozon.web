#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ozon.Simulator/Ozon.Simulator.csproj", "Ozon.Simulator/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Ozon.Bus/Ozon.Bus.csproj", "Ozon.Bus/"]
RUN dotnet restore "Ozon.Simulator/Ozon.Simulator.csproj"
COPY . .
WORKDIR "/src/Ozon.Simulator"
RUN dotnet build "Ozon.Simulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ozon.Simulator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozon.Simulator.dll"]