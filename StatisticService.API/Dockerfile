#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["StatisticService.API/StatisticService.API.csproj", "StatisticService.API/"]
COPY ["StatisticService.BLL/StatisticService.BLL.csproj", "StatisticService.BLL/"]
RUN dotnet restore "./StatisticService.API/StatisticService.API.csproj"
COPY . .
WORKDIR "/src/StatisticService.API"
RUN dotnet build "./StatisticService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StatisticService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StatisticService.API.dll"]
