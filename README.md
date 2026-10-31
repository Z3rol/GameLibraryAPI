# GameLibraryAPI

A REST API for tracking a personal video game library — browse a game catalog, write reviews, and manage your own collection. Built with ASP.NET Core and EF Core as a learning project, with a focus on doing the fundamentals properly: real authentication and authorization, layered architecture, structured logging, and a genuine automated test suite.

> I built this to properly learn backend development — starting from a tutorial project, then extending it well past what the tutorial covered: real authentication and role-based authorization, filtering/sorting/pagination across every resource, structured logging, a full automated test suite, and a handful of real bugs I found and fixed along the way (a leaked signing key, a race condition on duplicate reviews, an N+1 query). It's less "finished product" and more "everything I learned, applied."

## Features

- **JWT authentication** via ASP.NET Identity, with role-based authorization (`User` / `Admin`)
- **Games catalog** — browse, filter (by name, genre, developer, release date range, average rating), sort, and paginate
- **Reviews** — one review per user per game (enforced at both the application and database level), with a computed average rating per game
- **Personal library** — add/remove games, browse your own collection with the same filtering and sorting as the main catalog
- **Admin tooling** — a seeded admin account on first run, plus a role-gated endpoint to promote other users
- **Rate limiting** on login/registration to slow down brute-force attempts
- **Structured logging** with Serilog (console + rolling daily log files)
- **Global exception handling** returning consistent, sanitized error responses
- **Swagger UI**, including built-in support for testing authenticated endpoints with a JWT
- **Automated tests** — controller tests (mocked dependencies) and repository tests (real queries against an in-memory database)

## Tech stack

- **ASP.NET Core** (C#) / **Entity Framework Core**
- **PostgreSQL** for storage, run locally with Docker Compose
- **ASP.NET Identity** + **JWT Bearer** authentication
- **Serilog** for logging
- **Swashbuckle** (Swagger/OpenAPI)
- **xUnit** + **Moq** + **EF Core InMemory** for testing

## Architecture

```
Controllers/     → HTTP endpoints, request/response handling
Repository/      → data access, implements Interfaces/
Interfaces/      → repository contracts
DTOs/            → request/response shapes, separate from EF entities
Mappers/         → entity <-> DTO conversion
Models/          → EF Core entities
Helpers/         → query objects (filtering/sorting/pagination)
Middleware/      → global exception handling
Services/        → JWT token generation
Extensions/      → claims-reading helpers
Migrations/      → EF Core migration history
```

Standard layered structure: controllers depend on repository interfaces (not concrete classes), DTOs keep the API's public shape decoupled from the database schema, and query objects centralize filtering/sorting/pagination logic so it's consistent across endpoints.

## Getting started

**Prerequisites:** .NET SDK, Docker (for the PostgreSQL database), `dotnet-ef` tool (`dotnet tool install --global dotnet-ef`)

1. Clone the repo:
   ```bash
   git clone https://github.com/Z3rol/GameLibraryAPI.git
   cd GameLibraryAPI
   ```

2. Start the database. Copy `.env.example` to `.env`, set your own values, then start PostgreSQL with Docker Compose (data is kept in a Docker volume, so it survives restarts):
   ```bash
   cp .env.example .env
   docker compose up -d
   ```

3. Restore dependencies and set the required secrets (never committed — see `.gitignore`). The username, password and database in the connection string must match the values in your `.env`:
   ```bash
   cd GameLibraryAPI
   dotnet restore
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=<POSTGRES_DB>;Username=<POSTGRES_USER>;Password=<POSTGRES_PASSWORD>"
   dotnet user-secrets set "JWT:SigningKey" "<a long random string>"
   dotnet user-secrets set "Seed:AdminEmail" "admin@example.com"
   dotnet user-secrets set "Seed:AdminPassword" "<a strong password>"
   ```

4. Apply migrations and run:
   ```bash
   dotnet ef database update
   dotnet run
   ```

5. Open `/swagger` to explore and test the API. Log in via `/api/account/login` with your seeded admin credentials, then use the "Authorize" button in Swagger to test authenticated endpoints.

## API overview

| Area | Endpoints |
|---|---|
| **Account** | `POST /api/account/register`, `POST /api/account/login`, `POST /api/account/promote/{username}` *(Admin)* |
| **Games** | `GET /api/game`, `GET /api/game/{id}`, `POST /api/game` *(Admin)*, `PUT /api/game/{id}` *(Admin)*, `DELETE /api/game/{id}` *(Admin)* |
| **Reviews** | `GET /api/review/game/{gameId}`, `GET /api/review/user/{username}`, `POST /api/review`, `PUT /api/review/{gameId}`, `DELETE /api/review/{gameId}` |
| **Library** | `GET /api/users/{username}/library`, `POST /api/library/{gameId}`, `DELETE /api/library/{gameId}` |

Full request/response shapes and try-it-out testing are available in Swagger UI once running.

## Running tests

```bash
cd GameLibraryAPI.Tests
dotnet test
```

Tests are split into two layers:
- **Controller tests** mock repository dependencies to verify request handling, authorization flow, and response codes in isolation.
- **Repository tests** run real queries against an EF Core in-memory database, verifying filtering, sorting, aggregate calculations, and data-integrity logic that mocking can't exercise.

## Known limitations / what I'd add next

- No integration tests for `[Authorize(Roles = "Admin")]` enforcement — that's ASP.NET Core middleware behavior, which can't be exercised by calling controller methods directly the way the current tests do; would need a `WebApplicationFactory`-based integration test suite.
- No dedicated test for JWT construction (`TokenService`) — currently only exercised indirectly through mocked controller tests.
- Game deletion's cascade behavior on related reviews/library entries relies on EF Core's default behavior rather than an explicit, deliberate policy.
- Refresh tokens aren't implemented — JWTs are valid for their full lifetime with no revocation mechanism.

## License

MIT License
