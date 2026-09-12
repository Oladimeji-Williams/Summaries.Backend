# Production Guide

This document is the operational contract for running Summaries safely in a production environment.

## Release Gate

Before deployment, confirm all of the following:

- `dotnet build Summaries.slnx --configuration Release` succeeds with warnings treated as errors.
- `dotnet test Summaries.slnx --configuration Release --no-restore` succeeds in CI.
- Database migrations have been reviewed and a backup/restore plan exists.
- The production JWT signing secret is generated, stored in a secret manager, and is not present in repository files.
- `Frontend__BaseUrl` points to the exact HTTPS origin used by the client.
- CORS allows only known client origins.
- Resend sender identity and Cloudinary delivery URLs are verified.
- Paystack webhook signatures are validated and webhook endpoints are reachable over HTTPS.
- Logs and health signals are connected to the production monitoring system.
- A rollback artifact and rollback owner are identified.

## Configuration

Configuration is supplied through normal ASP.NET Core providers. Prefer environment variables or a managed secret store in production. Use double underscores for nested keys, for example `Jwt__Secret` and `ConnectionStrings__DefaultConnection`.

Important settings include:

| Setting | Purpose | Secret | Operational note |
| --- | --- | --- | --- |
| `ConnectionStrings__DefaultConnection` | SQL Server connection | Yes | Use least-privilege credentials. |
| `Jwt__Issuer` | Token issuer | No | Must match token validation. |
| `Jwt__Audience` | Token audience | No | Must match the deployed client contract. |
| `Jwt__Secret` | Signs access tokens | Yes | Rotate with a planned session invalidation strategy. |
| `Cors__AllowedOrigins` | Browser origins allowed by API | No | Do not use `*` with authenticated browser traffic. |
| `Frontend__BaseUrl` | Client callback and link base | No | Must be HTTPS outside local development. |
| `Resend__ApiKey` | Transactional email provider | Yes | Restrict access to the API process. |
| `Branding__LogoUrl` | Logo used by email templates | No | Use a stable HTTPS asset URL. |
| `Cloudinary__CloudName` | Media provider account | No | Keep media URLs stable across releases. |
| `Paystack__SecretKey` | Payment provider integration | Yes | Never log this value. |

Do not put secrets in `appsettings.json`, `appsettings.Development.json`, test snapshots, generated project exports, or exception messages.

## Deployment Sequence

1. Build and test the immutable release artifact in CI.
2. Apply reviewed database migrations using the deployment identity.
3. Deploy the API with production configuration injected by the platform.
4. Confirm startup, database connectivity, authentication, and provider health.
5. Exercise one non-destructive API smoke test and one transactional email test.
6. Monitor error rate, latency, rate-limit rejections, email failures, and payment webhook failures.

Keep migrations backward compatible with the previous application version when rolling deployments are possible. Avoid renaming or removing a database column in the same release that removes its last application reader.

## Security Baseline

The API currently applies HTTPS redirection, security headers, authentication, authorization, CORS, and rate limiting. Production review must additionally verify:

- TLS termination and forwarded-header configuration are correct at the edge.
- Access and refresh tokens are not written to logs.
- Authentication and password-reset endpoints have tested abuse limits.
- Uploaded avatars are content-validated, size-limited, and stored outside executable paths.
- Download URLs are authorized and time-bounded where the provider supports it.
- Error responses do not disclose provider credentials, SQL details, stack traces, or token material.
- Database users and cloud provider keys have the smallest practical permission set.

## Observability

Capture structured logs with a correlation identifier for each request. Recommended dashboards include:

- request count, latency percentiles, and 4xx/5xx rate by route;
- authentication failures, lockouts, refresh-token failures, and rate-limit rejections;
- database command duration and failed connection count;
- Resend, Cloudinary, and Paystack failure rates;
- background or webhook processing lag where applicable.

Alerts should be actionable and tied to an owner. A provider outage should degrade the affected workflow without exposing secrets or taking unrelated API capabilities offline.

## Backups and Recovery

Back up the SQL database according to the business recovery point objective. Test restoration, not only backup creation. Document the recovery point, recovery time objective, migration version, secret restoration path, and the person authorized to declare recovery.

## Incident Checklist

1. Record the UTC start time, affected route or provider, and current deployment version.
2. Check recent releases, configuration changes, logs, provider status, and database health.
3. Reduce blast radius with a rollback or feature/provider disablement when available.
4. Preserve relevant request identifiers and provider response codes without collecting credentials.
5. Communicate user impact and next update time.
6. After recovery, add a regression test, alert, runbook step, or code fix for the failure mode.