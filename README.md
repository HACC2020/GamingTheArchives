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
# From the archive-site-backend/src/ArchiveSite.Web folder run:
dotnet run -- init
```

Run the web host:

```bash
# From the archive-site-backend/src/ArchiveSite.Web folder run:
dotnet run
```