# EventsManager

A full-stack event management application built with ASP.NET Core (Web API), Entity Framework Core, and a React + TypeScript client. The project includes authentication, event and participant management, and is configured to use PostgreSQL via EF Core migrations.

## Key features

- REST API built with ASP.NET Core
- EF Core for data access and migrations
- React + TypeScript client (Client/)
- JWT authentication support
- Swagger for API documentation (when running the server in Development)

## Tech stack

- Backend: .NET 8, ASP.NET Core Web API, EF Core
- Database: PostgreSQL (configured via `appsettings.json`)
- Frontend: React, TypeScript, MobX (client code in `Client/`)

## Prerequisites

- .NET 8 SDK
- Node.js (16+ recommended)
- PostgreSQL (or another database supported by EF Core; change the connection string accordingly)
- Optional: Docker (to run the app or DB in containers)

## Quick start — local (recommended for development)

1. Clone the repository and change into the folder:

```bash
git clone <repo-url>
cd EventsManager
```

2. Configure the database connection. Edit `appsettings.json` (root) and set `ConnectionStrings:DefaultConnection` to point to your PostgreSQL instance.

3. Apply EF Core migrations to create the database schema:

```bash
# from repository root
dotnet ef database update
```

4. Install and start the frontend client:

```bash
cd Client
npm install
npm run start
```

The React client typically runs on http://localhost:3000.

5. Run the backend API from the repository root:

```bash
# from repository root
dotnet run
```

By default the API runs on a Kestrel port (earlier dev used 5274); check the console output or `Properties/launchSettings.json` for the exact port. If the API uses a different port, update the client API base URL in `Client/src/http/index.ts` accordingly.

6. Open the browser:

- Frontend: http://localhost:3000/
- Swagger UI (API docs): the server console will show the Swagger URL (commonly at http://localhost:<api-port>/swagger)

## Running with Docker

The repository contains a `Dockerfile`. You can build and run the app container (you still need a Postgres instance accessible to the container):

```bash
# build image
docker build -t eventsmanager:latest .

# run container (example; adapt env and ports)
docker run --rm -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Username=postgres;Password=yourpass;Database=eventsdb" -p 5274:5274 eventsmanager:latest
```

Alternatively run both app and database with docker-compose if you add a compose file that includes a postgres service.

## Tests

Run unit/integration tests from the repository root:

```bash
dotnet test
```

## Project layout (important folders)

- `Application/`, `Domain/`, `Infrastructure/` — typical layered solution projects
- `Controllers/` — API controllers (EventsController, ParticipantsController)
- `Client/` — React + TypeScript frontend
- `Migrations/` — EF Core migrations
- `Services/` — backend services and interfaces
- `Middlewares/` — exception handling middleware and custom exceptions

## Configuration notes

- Edit `appsettings.json` to set the DB connection string and other settings.
- If the client cannot reach the API because of a non-default port, change the API base URL in `Client/src/http/index.ts`.
- The project targets .NET 8 (see `bin/Debug/net8.0` in the repo). Adjust your SDK if required.

## Troubleshooting

- If EF migrations fail: ensure your connection string is correct and Postgres is reachable.
- If ports conflict: verify running processes or change ports in `launchSettings.json` and the client config.
- If npm install fails: delete `Client/node_modules` and try `npm ci` (if package-lock.json exists) or upgrade Node.js.

## Contributing

Feel free to open issues or pull requests. Follow the existing code style and add tests for significant behavior changes.

## License

This repository includes a `LICENSE` file. Check it for license terms.

---

Made by **ULtaR**
