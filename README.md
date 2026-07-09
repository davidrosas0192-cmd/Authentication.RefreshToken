# Authentication.RefreshToken

A RESTful ASP.NET Core 10 API with controllers, EF Core, JWT-based authentication, rotating refresh tokens, and one active session per username.

## What is included
- User registration endpoint
- Login, refresh, and logout endpoints
- Access token and refresh token issuance
- EF Core persistence with a local SQL Server database
- Migrations and implementation documentation

## Quick start
1. Start a local SQL Server instance and ensure the database exists.
2. Restore packages and build the API:
   ```bash
   dotnet build
   ```
3. Apply the migration at the end of the implementation:
   ```bash
   dotnet ef database update
   ```
4. Run the API:
   ```bash
   dotnet run
   ```

See [IMPLEMENTATION_PLAN.md](IMPLEMENTATION_PLAN.md) for the implementation plan and [Authentication.RefreshToken/README.md](Authentication.RefreshToken/README.md) for endpoint details.