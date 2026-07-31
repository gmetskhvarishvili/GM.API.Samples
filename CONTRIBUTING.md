# Contributing to GM.API Samples

This repository is a **usage sample** for the [`GM.API`](https://www.nuget.org/packages/GM.API)
stack. It is not published to NuGet — there is no versioning or release workflow here. The goal is
to keep the sample building, tested, and easy to follow.

## Prerequisites

- **.NET 10 SDK**
- **PostgreSQL** to run the API (tests use SQLite and need no database).

```bash
dotnet build -c Release
dotnet test  -c Release
```

## Branch & PR flow

1. Branch off `master`: `git switch -c fix/sample-query`
2. Open a PR into `master`. CI (`build` + tests) must pass.
3. Keep changes focused and the README in sync with what the sample does.

## Guidelines

- Reference the published **GM.*** packages — don't add project references into the library repos.
- Handler behaviour changes should come with a test in `tests/GM.API.Sample.Tests` (SQLite-backed).
- Bump a `GM.*` package version when a new release adds something the sample should show.

## Where releases happen

Package versioning, tags, changelog and nuget.org publishing live in the library repositories
(e.g. [`GM.API`](https://github.com/gmetskhvarishvili/GM.API)), driven by Conventional Commits.
Nothing is published from this samples repo.

## Code style

Enforced by [`.editorconfig`](.editorconfig). Run `dotnet format` before pushing if unsure.
