No migrations here yet. `PaymentsDbContext` is new — it replaces part of the
old combined `ApplicationDbContext` (see docs/MODULARIZATION.md, "Persistence:
three DbContexts now, not two"). Generate the first migration locally:

    dotnet ef migrations add InitialPayments --project src/Summaries.Modules.Payments --context PaymentsDbContext

If applying against a database that already has the old combined schema,
review the generated migration before running it — see the doc above.
