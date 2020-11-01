# Gaming The Archives

This is a web application to host archive transcription projects and provide an interactive user interface for volunteers to participate in document transcription. This project was created for the 2020 Hawaii Annual Coding Challenge.

## Prerequisites

* [Dotnet Core SDK 3.1](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/)
* [Docker Compose](https://docs.docker.com/compose/install/)

## Quick Start

Run services (postgresql) using `docker-compose`

```bash
# From the archive-site-backend folder run:
docker-compose up -d
```

Initialize your local database:

```bash
# From the archive-site-backend/src/ArchiveSite.Api folder run:
dotnet run -- init
```

Run the backend api host:

```bash
# From the archive-site-backend/src/ArchiveSite.Api folder run:
dotnet run
```

## Backend API

The backend API is an OData REST API that uses JSON payloads. The OData standard provides a flexible query language to efficiently select and filter data via the API. For details see [odata.org](https://www.odata.org/). By default the API is hosted at `http://localhost:5000`.

At a very high level, OData provides different endpoints for different resources (i.e. tables in the database), and for each endpoint there is a standard way of creating, reading, updating, and deleting records:

* `GET /api/Resource` - Returns a list of all records.
* `GET /api/Resource?$filter=Property+eq+'Value'` - Returns a filtered list of records.
* `GET /api/Resource?$select=Property1,Property2` - Returns only the specified properties for each record.
* `GET /api/Resource(Id)` - Returns the record with the specified id.
* `POST /api/Resource` - Given a body containing a JSON representation of the resource, creates a new record and returns the result.
* `PUT /api/Resource(Id)` - Updates an existing record with the specified Id. (Note: this completely overwrites the resource and may undo concurrent changes. The standard has a way to deal with this but I haven't implemented it).
* `PATCH /api/Resource(Id)` - Given a body containing a partial JSON representation of the resource, updates only the properties specified.
* `DELETE /api/Resource(Id)` - Deletes the specified record by id.

### Endpoints

* `/api/Users` - Registered user
    * TODO `/api/Users/Current` - Get the currently logged in user.
* `/api/Projects`
    * TODO `/api/Projects(Id)/Documents` - Get the documents associated with the specified project.
    * TODO `/api/Projects(Id)/Fields` - Get the fields for the specified project.
* `/api/Fields`
* `/api/Documents`
    * TODO `/api/Documents(Id)/Actions` 
    * TODO `/api/Documents(Id)/Notes` 
    * TODO `/api/Documents(Id)/Transcriptions` 
* `/api/DocumentActions`
* `/api/DocumentNotes`
* `/api/Transcriptions`

### Authentication

_TODO!_