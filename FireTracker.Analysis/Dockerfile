FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["../FireTracker.ServiceDefaults/FireTracker.ServiceDefaults.csproj", "FireTracker.ServiceDefaults/"]
COPY ["FireTracker.Analysis/FireTracker.Analysis.csproj", "FireTracker.Analysis/"]
RUN dotnet restore "FireTracker.Analysis/FireTracker.Analysis.csproj"

COPY . .
WORKDIR "/src/FireTracker.Analysis"
RUN dotnet build "FireTracker.Analysis.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FireTracker.Analysis.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FireTracker.Analysis.dll"]
