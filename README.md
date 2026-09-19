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
Email__ApiKey=your-resend-key
Jwt__SecretKey=use-a-long-random-development-secret
Jwt__Issuer=Summaries.API
Jwt__Audience=Summaries.Client
Jwt__AccessTokenExpirationMinutes=15
Jwt__RefreshTokenExpirationDays=7
Email__FromAddress=onboarding@resend.dev
Email__FromName=Summaries
Paystack__SecretKey=your-paystack-secret
Cloudinary__CloudName=your-cloud-name
Cloudinary__ApiKey=your-cloudinary-key
Cloudinary__ApiSecret=your-cloudinary-secret
Branding__LogoUrl=https://example.com/logo.png
Cors__AllowedOrigins__0=http://localhost:4200
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

The solution contains per-module unit test projects, API integration tests, and architecture tests. Integration tests may require SQL Server and external-service configuration.

## Architecture

The solution is a vertical-slice modular monolith: each feature module owns
its full stack (Api/Application/Domain/Infrastructure/Persistence, as
applicable), not just its use cases. See `docs/MODULARIZATION.md` for the
full picture, including the cross-module contracts pattern and a note on
persistence migrations.

- `Summaries.SharedKernel`: the innermost layer — `Entity` base type, `Result`/`Error` primitives, the MediatR validation pipeline behavior, port interfaces (`Abstractions`) implemented by each module's own Infrastructure/Persistence, and `Contracts` — small read-only DTOs/errors one module exposes for another to consume. Depends on nothing else in the solution.
- `Summaries.Shared.Infrastructure`: generic technology every module might use but none of them owns — email, file storage, image validation, current-user, URL building, and the shared API base-controller classes.
- `Summaries.Modules.Authentication`: login, registration, 2FA, external providers, and password reset. Owns the ASP.NET Identity user record end to end.
- `Summaries.Modules.Books`: catalog and reading progress. Owns `BooksDbContext`.
- `Summaries.Modules.Payments`: Paystack checkout and purchase verification. Owns `PaymentsDbContext`.
- `Summaries.Modules.Users`: profile management, avatars, 2FA setup. No persistence of its own — reads/writes through `IIdentityService`.
- `Summaries.Modules.Admin`: cross-cutting reporting over users and books. No persistence of its own — reads through `IIdentityService` and Books' `IBookCatalog`/`IBookReadingHistory` contracts.
- `Summaries.API`: composition root only — DI wiring, versioning, CORS, security headers, rate limiting. No controllers; those live in each module's own `Api/` folder.
- `tests`: unit tests per module, plus API integration tests and architecture tests enforcing both the no-module-depends-on-another rule and the layering within each module.
- `tools/Summaries.DatabaseSeeder`: repeatable local data setup.

Dependencies point inward, and feature modules never point sideways at each
other. Controllers should orchestrate HTTP concerns and delegate business
behavior to module requests. Infrastructure implementations should be
registered in the composition root rather than referenced directly by
feature modules. `tests/Summaries.ArchitectureTests` enforces all of this at
build time.

## API Conventions

- API controllers are versioned under `Controllers/V1`.
- Request and response contracts live near their application feature.
- Validation failures and domain failures should use the existing `Result` and validation pipeline conventions.
- Authentication uses short-lived access tokens and refresh tokens.
- Auth endpoints are rate limited by the API rate-limiter policy.
- External login callbacks use `Frontend__BaseUrl`; keep that value aligned with the deployed client origin.

## Documentation Map

- [Modularization](docs/MODULARIZATION.md): the module dependency graph, SharedKernel Contracts pattern, and what the architecture tests enforce.
- [Production guide](docs/PRODUCTION_GUIDE.md): deployment, configuration, security, observability, and release readiness.
- [Brand system](docs/BRAND_SYSTEM.md): visual tokens and implementation rules for email and future client applications.

## Common Failure Modes

- `Failed to determine the https port for redirect`: run with the HTTPS launch profile or configure an explicit HTTPS endpoint.
- Resend returns `403`: verify the API key, sender identity, and account permissions.
- Cloudinary host or asset failures: verify DNS/network access and the configured Cloudinary URL.
- Database connection failures: verify SQL Server is running and the development connection string matches the local instance.

## Contribution Standards

Keep changes focused, preserve layer boundaries, add or update the nearest test, and run formatting/build/tests before opening a pull request. Treat configuration changes as deployment changes: document new settings, defaults, secret requirements, and rollback behavior.