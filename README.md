# Summaries Backend

Summaries is a .NET 10 backend for a reading platform. It exposes versioned HTTP APIs for authentication, profiles, books, reading progress, purchases, payments, file delivery, and transactional email.

This repository is the backend only. There is no web client, component library, or frontend stylesheet in this workspace. The API owns the product brand contract and the transactional email presentation; the client application should consume the contract described in `docs/BRAND_SYSTEM.md`.

## Quick Start

### Prerequisites

- .NET SDK 10.x
- SQL Server or SQL Server Express
- A Resend API key for email flows
- Cloudinary credentials when using remote book or avatar storage
- Paystack credentials when using payments

### Configure local secrets

Keep secrets outside source control. The application loads `.env` from the repository root during startup. A minimal local configuration looks like this:

```text
ConnectionStrings__DefaultConnection=Server=localhost;Database=SummariesDb;Trusted_Connection=True;TrustServerCertificate=True;
Resend__ApiKey=your-resend-key
Jwt__Secret=use-a-long-random-development-secret
Paystack__SecretKey=your-paystack-secret
Cloudinary__ApiKey=your-cloudinary-key
Cloudinary__ApiSecret=your-cloudinary-secret
Frontend__BaseUrl=http://localhost:4200
```

Use the existing `appsettings.Development.json` only for non-secret local defaults. Never commit API keys, signing secrets, or production connection strings.

### Run

```powershell
dotnet restore Summaries.slnx
dotnet build Summaries.slnx
dotnet run --project src\Summaries.API
```

The default HTTP profile is `http://localhost:5079`. Development OpenAPI metadata is available from the API host when enabled by the current environment.

### Test

```powershell
dotnet test Summaries.slnx --no-restore
```

The solution contains unit, integration, architecture, persistence, and infrastructure test projects. Integration tests may require SQL Server and external-service configuration.

## Architecture

The solution follows a layered architecture:

- `Summaries.Domain`: entities, value-independent domain rules, enums, and shared domain primitives.
- `Summaries.Application`: use cases, MediatR requests, validation, DTOs, and ports for infrastructure concerns.
- `Summaries.Persistence`: EF Core context, migrations, configurations, repositories, and database access.
- `Summaries.Infrastructure`: identity, email, storage, payment, and external-provider integrations.
- `Summaries.API`: HTTP controllers, versioning, CORS, security headers, rate limiting, and composition root.
- `tests`: behavior, integration, architecture, and provider-focused verification.
- `tools/Summaries.DatabaseSeeder`: repeatable local data setup.

Dependencies point inward. Controllers should orchestrate HTTP concerns and delegate business behavior to application requests. Infrastructure implementations should be registered in the composition root rather than referenced directly by application features.

## API Conventions

- API controllers are versioned under `Controllers/V1`.
- Request and response contracts live near their application feature.
- Validation failures and domain failures should use the existing `Result` and validation pipeline conventions.
- Authentication uses short-lived access tokens and refresh tokens.
- Auth endpoints are rate limited by the API rate-limiter policy.
- External login callbacks use `Frontend__BaseUrl`; keep that value aligned with the deployed client origin.

## Documentation Map

- [Production guide](docs/PRODUCTION_GUIDE.md): deployment, configuration, security, observability, and release readiness.
- [Brand system](docs/BRAND_SYSTEM.md): visual tokens and implementation rules for email and future client applications.

## Common Failure Modes

- `Failed to determine the https port for redirect`: run with the HTTPS launch profile or configure an explicit HTTPS endpoint.
- Resend returns `403`: verify the API key, sender identity, and account permissions.
- Cloudinary host or asset failures: verify DNS/network access and the configured Cloudinary URL.
- Database connection failures: verify SQL Server is running and the development connection string matches the local instance.

## Contribution Standards

Keep changes focused, preserve layer boundaries, add or update the nearest test, and run formatting/build/tests before opening a pull request. Treat configuration changes as deployment changes: document new settings, defaults, secret requirements, and rollback behavior.