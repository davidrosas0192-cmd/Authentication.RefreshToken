# Authentication.RefreshToken API

This project implements a RESTful ASP.NET Core 10 API with controllers, EF Core, JWT authentication, refresh tokens, and one active session per username.

## Main features
- User registration
- Login endpoint issuing access and refresh tokens
- Refresh token rotation
- Logout endpoint revoking the current session
- Single active session per username
- SQL Server-backed persistence with EF Core migrations

## Endpoints
- POST /api/users
- POST /api/auth/login
- POST /api/auth/refresh
- POST /api/auth/logout
- GET /api/health

## Local database setup
1. Ensure SQL Server is running locally on port 1433.
2. Make sure the database AuthenticationFido2 exists or is created by the migration.
3. Apply the migration:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```
