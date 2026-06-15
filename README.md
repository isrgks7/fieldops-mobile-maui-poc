# fieldops-mobile-maui-poc
Offline-first .NET MAUI proof of concept for field work order management, built with Clean Architecture, MVVM, SQLite, dependency injection, repository pattern, sync simulation, and unit tests.

## Features

- **Dashboard** - summary cards (total, open, in progress, completed, pending sync), last sync time, sync now
- **Work Orders list** - search, status filters, pending sync indicator, navigation to details
- **Work Order detail** - view full context, start work, complete with notes, save notes locally
- **Offline-first** - SQLite persistence; app works when sync fails
- **Simulated sync** - fake API with latency and configurable failure rate
- **Unit tests** - domain rules, use cases, repository seeding

## Folder structure

```
src/
  FieldOps.Domain/          Business entities and enums
  FieldOps.Application/     Use cases, DTOs, interfaces
  FieldOps.Infrastructure/  SQLite, repositories, fake API, seed data
  FieldOps.Mobile/          MAUI UI (Pages, ViewModels, Converters)
tests/
  FieldOps.Application.Tests/
  FieldOps.Infrastructure.Tests/
```

## Commands

Restore:

```bash
dotnet restore FieldOps.sln
```

Build (all libraries + tests):

```bash
dotnet build src/FieldOps.Domain/FieldOps.Domain.csproj
dotnet build src/FieldOps.Application/FieldOps.Application.csproj
dotnet build src/FieldOps.Infrastructure/FieldOps.Infrastructure.csproj
```

Build mobile app:

```bash
dotnet build src/FieldOps.Mobile/FieldOps.Mobile.csproj -f net10.0-windows10.0.19041.0
```

Run:

```bash
dotnet build src/FieldOps.Mobile/FieldOps.Mobile.csproj -t:Run -f net10.0-windows10.0.19041.0
```

Run (Android - requires Android SDK/emulator):

```bash
dotnet build src/FieldOps.Mobile/FieldOps.Mobile.csproj -t:Run -f net10.0-android
```

Run tests:

```bash
dotnet test tests/FieldOps.Application.Tests/FieldOps.Application.Tests.csproj
dotnet test tests/FieldOps.Infrastructure.Tests/FieldOps.Infrastructure.Tests.csproj
```
