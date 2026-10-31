<p align="center">
  <img src="icon.png" alt="GM.API Samples" width="140" height="140" />
</p>

# GM.API Samples

[![CI](https://github.com/gmetskhvarishvili/GM.API.Samples/actions/workflows/ci.yml/badge.svg)](https://github.com/gmetskhvarishvili/GM.API.Samples/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A runnable, layered **DDD + CQRS** ASP.NET Core Web API that shows how to build a real application on
**[GM.API](https://www.nuget.org/packages/GM.API)** — thin controllers, mediator commands/queries,
a domain model, and an EF Core + PostgreSQL persistence layer — together with
[GM.API.Application](https://www.nuget.org/packages/GM.API.Application),
[GM.EntityFramework](https://www.nuget.org/packages/GM.EntityFramework),
[GM.Mediator](https://www.nuget.org/packages/GM.Mediator) and
[GM.Mapper](https://www.nuget.org/packages/GM.Mapper). The Swagger UI (and the custom
`Assets/swagger.css` / `swagger.js`) comes from
[GM.Documentation](https://www.nuget.org/packages/GM.Documentation), which `GM.API` pulls in
transitively. Targets `net10.0`.

## Projects

```
GM.API.Samples/
├── GM.API.Sample.API/            # ASP.NET Core Web API — AddGMAPI/UseGMAPI, controllers, Swagger, Serilog
├── GM.API.Sample.Application/    # CQRS commands/queries + handlers (GM.Mediator), DTOs
├── GM.API.Sample.Domain/         # Sample aggregate + repository/unit-of-work contracts (GM.EntityFramework.Domain)
├── GM.API.Sample.Persistence/    # EF Core DbContext, configurations, repositories, migrations (PostgreSQL)
├── GM.API.Sample.Common/         # Shared resources / localized strings
└── tests/
    └── GM.API.Sample.Tests/      # SQLite-backed handler tests
```

Only the published **GM.*** packages are referenced — no project references into the library repos.

## What it demonstrates

- **`AddGMAPI` / `UseGMAPI`** wiring the whole request pipeline (CORS, versioning, Serilog,
  exception handling, request logging, localization) from `GM.API`, plus a versioned **Swagger UI**
  from `GM.Documentation` (pulled in transitively), skinned with the sample's `Assets/swagger.css` / `swagger.js`.
- **CQRS** — `CreateSample` / `UpdateSample` / `DeleteSample` commands and
  `GetSampleDetails` / `GetSamplesList` queries, dispatched through GM.Mediator.
- **DDD persistence** — a `Sample` aggregate with child `SampleItem`s over the GM.EntityFramework
  generic repository / unit-of-work / specification base, with soft-delete and audit stamping.
- **List paging** — `GetSamplesList` (on GM.API's `GetBaseListModel` / `GetBaseListQuery`) pages by
  `CurrentPage` (page 1 by default) or, when `LastKeyValue` is sent, returns the samples after the one
  identified by `LastKeyProperty` (`Id` by default) + `LastKeyValue`, in `OrderBy` order
  (e.g. `?LastKeyProperty=Name&LastKeyValue=B&OrderBy=Id%20desc`). `IncludeTotalCount=false` skips the
  count query. The body is the samples; `X-Pagination` carries `totalCount`, `pageSize`, `currentPage`
  (0 when paged by key), `totalPages`, `lastKeyProperty`, `lastKeyValue` and `hasMore`.
- **Mapster projection** to DTOs.
- **Health endpoints** — `/health/live` (process liveness, no downstream dependencies) and
  `/health/ready` (registered health checks), wired via the standard ASP.NET Core health check
  middleware.
- **Secrets** — `CreateGMBuilder` layers secrets into configuration (GM.Secrets): a gitignored
  `GM.API.Sample.API/appsecrets.json` overrides `appsettings.json` locally — e.g. put your real
  `ConnectionStrings:ApplicationDatabase` there — and when deployed `SECRETS_PROVIDER` loads them from
  Doppler, AWS Secrets Manager, Azure Key Vault or Vault (plus that store's own environment variables).
  The app code just reads configuration.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A **PostgreSQL** database (connection string in `GM.API.Sample.API/appsettings.json`). The app runs
  EF Core migrations and seeds on startup.

## Running

```bash
dotnet run --project GM.API.Sample.API
```

Open the app root (`/`) — it redirects to the versioned Swagger UI.

## Testing

```bash
dotnet test
```

The suite exercises the command and query handlers against a private **SQLite** in-memory database
(via `SampleTestHost`), so it runs anywhere without a PostgreSQL server.

## License

MIT — see [LICENSE](LICENSE).
