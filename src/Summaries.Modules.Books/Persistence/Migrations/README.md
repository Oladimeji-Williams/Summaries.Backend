No migrations here yet. `BooksDbContext` is new — it replaces part of the
old combined `ApplicationDbContext` (see docs/MODULARIZATION.md, "Persistence:
three DbContexts now, not two"). Generate the first migration locally:

    dotnet ef migrations add InitialBooks --project src/Summaries.Modules.Books --context BooksDbContext

If applying against a database that already has the old combined schema,
review the generated migration before running it — see the doc above.
