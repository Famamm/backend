# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file
COPY ["fleetops_backend.csproj", "./"]

# Restore dependencies
RUN dotnet restore "fleetops_backend.csproj"

# Copy all source code
COPY . .

# Build the application
RUN dotnet build "fleetops_backend.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "fleetops_backend.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ARG API_PORT=5144
ARG API_ENVIRONMENT=Production

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*
COPY --from=publish /app/publish .

EXPOSE ${API_PORT}

ENV ASPNETCORE_URLS=http://+:${API_PORT}
ENV ASPNETCORE_ENVIRONMENT=${API_ENVIRONMENT}

HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:${API_PORT}/health || exit 1

ENTRYPOINT ["dotnet", "fleetops_backend.dll"]
