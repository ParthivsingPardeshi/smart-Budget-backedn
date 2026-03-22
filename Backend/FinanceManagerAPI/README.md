# FinanceManagerAPI

## Database configuration

The API reads the database connection string from either:

- `DATABASE_URL` (recommended for hosted Postgres/Supabase), or
- `ConnectionStrings:DefaultConnection` (from `appsettings*.json` / environment variables)

Supported formats:

- PostgreSQL URL: `postgresql://user:password@host:5432/dbname`
- Npgsql connection string: `Host=...;Port=5432;Database=...;Username=...;Password=...`

### Example (PowerShell)

```powershell
$env:DATABASE_URL = "postgresql://user:password@host:5432/dbname"
dotnet run --project .\FinanceManagerAPI.csproj
```
