# Contributing to Beatok API

First off, thank you for considering contributing to Beatok!

## Prerequisites

**[.NET 10](https://dotnet.microsoft.com/)** is the framework used for the backend.

Make sure you have the .NET 10 SDK installed.

## Local Development

1. Fork and clone the repository.
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Apply database migrations:
   ```bash
   dotnet ef database update
   ```
4. Start the API:
   ```bash
   dotnet run
   ```
### Database

When making changes to the database model, create a new Entity Framework Core migration:

```bash
dotnet ef migrations add <MigrationName>
```

Apply the migration locally:

```bash
dotnet ef database update
```

## Pull Request Process

1. Create a descriptive branch name (e.g., `feat/add-leaderboard` or `fix/lobby-cleanup`).
2. Make your changes and test them locally.
3. Make sure the project builds successfully:
   ```bash
   dotnet build
   ```
4. Push your branch and open a Pull Request.

## Reporting Bugs and Features

Please use the provided issue templates to report bugs or request features. Include as much context as possible!
