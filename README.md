# Gaming The Archives

This is a web application to host archive transcription projects and provide an interactive user interface for volunteers to participate in document transcription. This project was created for the 2020 Hawaii Annual Coding Challenge.

## Prerequisites

* [Dotnet Core SDK 3.1](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/)
* [Docker Compose](https://docs.docker.com/compose/install/)
    * Windows systems do not need to install Docker Compose since it is already included in the Docker Desktop installation.

## Quick Start

Run services (Postgresql) using `docker-compose`

```bash
# From the archive-site-backend folder run:
docker-compose up -d
```

Initialize your local database:

```bash
# From the archive-site-backend/src/ArchiveSite.Api folder run:
dotnet run -- init
```

Run backend unit tests:

```bash
# From the archive-site-backend/src/ArchiveSite.Api.Tests folder run:
dotnet test
```

Run the backend api host:

```bash
# From the archive-site-backend/src/ArchiveSite.Api folder run:
dotnet run # Alternately you can use "dotnet watch run" to automatically rebuild
```


## Backend API

The backend API is an OData REST API that uses JSON payloads. The OData standard provides a flexible query language to efficiently select and filter data via the API. For details see [odata.org](https://www.odata.org/). By default the API is hosted at `http://localhost:5000`.

At a very high level, OData provides different endpoints for different resources (i.e. tables in the database), and for each endpoint there is a standard way of creating, reading, updating, and deleting records:

* `GET /api/odata/Resource` - Returns a list of all records.
* `GET /api/odata/Resource?$filter=Property+eq+'Value'` - Returns a filtered list of records.
* `GET /api/odata/Resource?$select=Property1,Property2` - Returns only the specified properties for each record.
* `GET /api/odata/Resource(Id)` - Returns the record with the specified id.
* `POST /api/odata/Resource` - Given a body containing a JSON representation of the resource, creates a new record and returns the result.
* `PUT /api/odata/Resource(Id)` - Updates an existing record with the specified Id. (Note: this completely overwrites the resource and may undo concurrent changes. The standard has a way to deal with this but I haven't implemented it).
* `PATCH /api/odata/Resource(Id)` - Given a body containing a partial JSON representation of the resource, updates only the properties specified.
* `DELETE /api/odata/Resource(Id)` - Deletes the specified record by id.

### Endpoints

* `/api/odata/Users` - Registered user
    * TODO `/api/odata/Users/Current` - Get the currently logged in user.
* `/api/odata/Projects`
    * TODO `/api/odata/Projects(Id)/Documents` - Get the documents associated with the specified project.
    * TODO `/api/odata/Projects(Id)/Fields` - Get the fields for the specified project.
* `/api/odata/Fields`
* `/api/odata/Documents`
    * TODO `/api/odata/Documents(Id)/Actions` 
    * TODO `/api/odata/Documents(Id)/Notes` 
    * TODO `/api/odata/Documents(Id)/Transcriptions` 
* `/api/odata/DocumentActions`
* `/api/odata/DocumentNotes`
* `/api/odata/Transcriptions`

### Authentication

_TODO!_

### Configuration

The backend API can be configured in several ways: appsettings json files, environment variables, and command line arguments. Configuration values are a hierarchical tree structure based on top level section names, and configuration class structures:

* Logging
    * LogLevel
        * Default - The default minimum log level
        * {LoggingSource} - A minimum log level override for each logging source (the fully qualified name of the class that is doing the logging)
* ArchiveDb
    * Host
    * Port
    * Database
    * Username
    * Password 
* OriginPolicy
    * Allow - A Glob syntax host name from which cross origin API requests should be allowed (i.e. `"*"` to allow all cross origin requests).
* Azure
    * ApiUrl - your Azure Cognitive Service URL
    * ApiKey - your Azure Cognitive Service API key

### Appsettings Files

Two appsettings files are loaded with the backend process starts: `appsettings.json` and `appsettings.{Environment}.json` where `{Environment}` is the string specified by the `--environment` command line argument or via the `ASPNETCORE_ENVIRONMENT` environment variable.

### Environment Variables

Configuration values can be overridden via environment variables with the following syntax for variable names: `{Section}__{Property}[__{SubProperty}]`. For example to override the default log level you could use the command line:

```bash
Logging__LogLevel__Default=Trace dotnet run
```

### Command Line

Lastly configuration values can be overridden via command line arguments. The syntax for command line argument overrides is `--{Section}:{Property}[:{SubProperty}]`. So to perform the same log level override as above you would use the following command line:

```bash
dotnet run -- --Logging:LogLevel:Default Trace
```