FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
RUN mkdir -p /app/src/ArchiveSiteBackend.Data && mkdir -p /app/src/ArchiveSiteBackend.Api
COPY ./src/ArchiveSiteBackend.Data/*.csproj ./src/ArchiveSiteBackend.Data
COPY ./src/ArchiveSiteBackend.Api/*.csproj ./src/ArchiveSiteBackend.Api

WORKDIR /app/src/ArchiveSiteBackend.Api
RUN dotnet restore

# Copy everything else and build
COPY ./src/ArchiveSiteBackend.Data /app/src/ArchiveSiteBackend.Data
COPY ./src/ArchiveSiteBackend.Api ./
RUN dotnet publish -c Release -o ../../out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "archive-api.dll"]
