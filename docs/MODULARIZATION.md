# Backend modularization

The solution is a **vertical-slice modular monolith**: each feature module is
one class library containing its own `Api`/`Application`/`Domain`/
`Infrastructure`/`Persistence` — not just its own use cases, as an earlier
pass of this refactor had it.

```
src/
  Summaries.API/                    composition root only — no controllers
  Summaries.SharedKernel/           innermost layer, depends on nothing
  Summaries.Shared.Infrastructure/  generic tech every module may use
  Summaries.Modules.Authentication/ Api Application Infrastructure Persistence
  Summaries.Modules.Books/          Api Application Domain         Persistence
  Summaries.Modules.Payments/       Api Application Domain Infrastructure Persistence
  Summaries.Modules.Users/          Api Application   (no Domain/Infra/Persistence)
  Summaries.Modules.Admin/          Api Application   (no Domain/Infra/Persistence)
```

A module only has the folders it needs. Users and Admin own no data of their
own, so they have neither — see "Cross-module contracts" below for why.

## Why Payments is its own module

`Purchase`, Paystack integration, and the initiate/verify commands used to
live inside Books. They're a genuinely separate concern — Books doesn't need
to know how a checkout works, and Payments doesn't need to know how a book
is catalogued — so they're now `Summaries.Modules.Payments`, with its own
`PaymentsDbContext`.

## Why Authentication owns the user record

Login, 2FA, and password resets are all credential-shaped operations on
`UserManager<ApplicationUser>` — inherently security-sensitive. Users'
profile-editing operations touch the same row but for a completely
different reason (display data, not security). Rather than let both
modules write to the same table, **Authentication owns `ApplicationUser`
and implements `IIdentityService`** (declared in SharedKernel); Users,
Admin, and Books consume that interface through DI and have no persistence
of their own for it. This is the same "ports and adapters" shape already
used for `IBookRepository` et al. — the abstraction lives centrally, the
implementation lives wherever owns the data, and consumers never need a
project reference to the implementer.

## Module dependency graph: fully independent

No module has a `ProjectReference` to another module. Each depends only on
`Summaries.SharedKernel` and `Summaries.Shared.Infrastructure`:

```
SharedKernel  (depends on nothing)
  ^
  +-- Shared.Infrastructure (implements SharedKernel abstractions)
  ^
  +-- Modules.Authentication
  +-- Modules.Books
  +-- Modules.Payments
  +-- Modules.Users
  +-- Modules.Admin       (all five: siblings, never see each other)
```

`tests/Summaries.ArchitectureTests/LayerRules/ModuleIsolationTests.cs`
enforces this at build time.

## Cross-module contracts (SharedKernel.Contracts)

A few things are genuinely needed by more than one module. Rather than let
one module reference another's project to get them, they're published as
narrow, **DTO-shaped** contracts in `SharedKernel.Contracts` — never the
actual entity or full repository:

- `Contracts.Users`: `UserProfileDto`, `UserErrors` — Users' own shape,
  used by Users itself and read by Admin.
- `Contracts.Books`: `BookStatus` (shared vocabulary — Admin displays
  reading status too), `BookSummaryDto` + `IBookCatalog` (read-only book
  lookup), `ReadingRecordSummaryDto` + `IBookReadingHistory` (read-only
  reading history), `BookErrors`.
- `Contracts.Payments`: `IPurchaseVerifier` — the one thing Books needs to
  know about purchases (has this user bought this book), without seeing
  Payments' own `IPurchaseRepository` or `Purchase` entity.

Each owning module implements its contract via a small adapter (e.g.
`Books.Application.CrossModule.BookCatalogService` wraps Books' own
repositories and maps entities to the shared DTOs) and registers it in its
own `DependencyInjection.cs`. **This is the pattern going forward**: if
module B needs something from module A, the shape moves to
`SharedKernel.Contracts.<A>` — B never takes a `ProjectReference` on A.

This replaced a real bug caught during this pass: `IBookRepository` and
`IPurchaseRepository` used to live in SharedKernel but returned the actual
`Book`/`Purchase` entities, which now live inside their modules — so
SharedKernel would have needed a reference to those modules to compile,
inverting the whole dependency direction. Both interfaces moved into the
modules that own them; the contracts above cover what other modules
actually needed.

## Module-internal layering

Since each module is one assembly now, the compiler can't stop `Books.Api`
from reaching directly into `Books.Persistence`, or `Books.Domain` from
reaching into `Books.Infrastructure` — that's enforced by
`tests/Summaries.ArchitectureTests/LayerRules/ModuleInternalLayeringTests.cs`
instead, using namespace conventions:

- `{Module}.Domain` must not depend on `{Module}.Application/Infrastructure/Persistence/Api`
- `{Module}.Application` must not depend on `{Module}.Infrastructure/Persistence/Api`
- `{Module}.Infrastructure`/`{Module}.Persistence` must not depend on `{Module}.Api`

Only modules that actually have a given layer get that rule (Users/Admin
have no Domain/Infrastructure/Persistence, so those checks are skipped for
them).

## Controllers moved into each module — a wiring detail

Every controller now lives in its module's own `Api/Controllers/` folder,
in that module's own assembly. ASP.NET Core's MVC only discovers
controllers from the entry assembly by default — it won't find controllers
in a separate class library unless that assembly is registered as an
`ApplicationPart`. `Program.cs` collects all five module assemblies and
passes them to `AddApiServices`, which forwards them to
`ControllerExtensions.AddApiControllers(...params Assembly[])`. Miss this
step and every module's endpoints silently 404.

`ApiControllerBase`/`V1ControllerBase`/`ApiResponse`/`ApiError` — the
pieces every module's `Api/` layer needs — live in
`Summaries.Shared.Infrastructure.Api`, since they're ASP.NET-specific and
SharedKernel is deliberately framework-free.

## Persistence: three DbContexts now, not two

- `ApplicationIdentityDbContext` (Authentication) — unchanged, just
  relocated. Its migration history was a like-for-like move: I updated the
  fully-qualified type-name **string literals** EF Core embeds in
  `*.Designer.cs`/`*ModelSnapshot.cs` (e.g.
  `Summaries.Infrastructure.Identity.ApplicationUser` →
  `Summaries.Modules.Authentication.Infrastructure.Identity.ApplicationUser`)
  to match the entities' new namespace. This is safe because it's a pure
  rename — the same context, same tables, same history, nothing split.
- `BooksDbContext` (Books) and `PaymentsDbContext` (Payments) are **new** —
  they replace the old combined `ApplicationDbContext`, which tracked
  `Book`, `BookReadingRecord`, and `Purchase` together under one migration
  history. **This one is not a pure rename — it's a real context split,
  and I did not attempt to hand-generate migrations for it.** The old
  migration history is archived at
  `docs/legacy-migrations/pre-split-books-payments/` for reference, and
  each new module has an empty `Persistence/Migrations/` folder with its
  own `README.md`. You'll need to run, locally, with the EF CLI tools and
  a reachable database:
  ```
  dotnet ef migrations add InitialBooks --project src/Summaries.Modules.Books --context BooksDbContext
  dotnet ef migrations add InitialPayments --project src/Summaries.Modules.Payments --context PaymentsDbContext
  ```
  If you're applying this against a database that already has the old
  combined schema (rather than a fresh dev database), don't just
  `dotnet ef database update` — the generated migration will try to
  `CREATE TABLE` tables that already exist. You'll want to review the
  generated migration and either strip the `CreateTable` calls (schema
  already matches) or mark the migration as applied by hand-inserting its
  row into each new `__EFMigrationsHistory_*` table without running its
  `Up()`.

## Composition root

Each module has an `AssemblyMarker` class used to obtain its assembly for
MediatR/FluentValidation/ApplicationPart registration. The single MediatR
pipeline (with the shared `ValidationBehavior`) is wired **once**, in
`Summaries.SharedKernel.DependencyInjection.AddSharedKernel(...)`, called
from `Program.cs` with all five module assemblies — this avoids running
the validation behavior once per module.

## What still needs a manual pass

- **Migrations for Books/Payments**, as above — the one piece of this
  refactor that genuinely requires tooling I don't have in this sandbox
  (a reachable SQL Server instance).
- **No `dotnet restore` was possible here at all this time** — this pass
  moved `Entity` out of the old dependency-free `Summaries.Domain` project
  into `SharedKernel`, which needs MediatR/FluentValidation from
  `api.nuget.org` (not on this sandbox's network allowlist). Every
  previous pass at least got one project building; this one didn't. What
  I *did* do instead, exhaustively, after every batch of changes:
  - every `ProjectReference` in every `.csproj` resolves to a real file
  - every `using`/`global using` resolves to a namespace actually declared
    somewhere in the solution
  - zero duplicate `using` lines (a real `TreatWarningsAsErrors` risk —
    caught two of these from namespace collisions during the rename)
  - zero leftover references to any of the four projects this refactor
    deleted (`Summaries.Domain`, `.Infrastructure`, `.Persistence`,
    `.Application`)

  This is real signal, but it is not a compiler — please run
  `dotnet restore && dotnet build` locally as the first real check.
- Two bugs specific to this pass that a compiler would have caught
  immediately, and that I found by inspection instead — worth knowing
  about in case anything similar slipped through: a stale
  `Controllers.Base.ApiControllerBase` qualifier left over from when that
  class lived in a different namespace, and a missing
  `public partial class Program;` marker (top-level statements don't
  expose one by default, and `Summaries.API.IntegrationTests`'
  `WebApplicationFactory<Program>` needs it). Both are fixed, but it's a
  reminder that hand-verification has gaps a real build doesn't.
