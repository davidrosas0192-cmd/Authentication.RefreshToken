# Authentication.RefreshToken API

This project implements a RESTful ASP.NET Core 10 API with controllers, EF Core, JWT authentication, refresh tokens, and one active session per username.

## Main features
- User registration
- Login endpoint issuing access and refresh tokens
- Refresh token rotation
- Logout endpoint revoking the current session
- Single active session per username
- SQL Server-backed persistence with EF Core migrations
- Refresh tokens are hashed with HMAC-SHA256 using a server-side secret, following OWASP guidance for secure refresh-token storage

## Endpoints
- POST /api/users
- POST /api/auth/login
- POST /api/auth/refresh
- POST /api/auth/refresh-access
- POST /api/auth/logout
- GET /api/health

## Request examples

### 1. Create a user
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Content-Type: application/json" \
  -d '{"userName":"demo","password":"123456"}'
```

### 2. Login and receive tokens
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"userName":"demo","password":"123456"}'
```

Example response:
```json
{
  "accessToken": "<jwt-access-token>",
  "refreshToken": "<refresh-token>",
  "accessTokenExpiresAt": "2026-07-08T10:00:00Z",
  "refreshTokenExpiresAt": "2026-07-15T10:00:00Z"
}
```

### 3. Refresh the session with the refresh token
```bash
curl -X POST http://localhost:5000/api/auth/refresh \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"<refresh-token-from-login>"}'
```

This endpoint returns a new access token and a new refresh token. The previous refresh token is invalidated, which enforces refresh-token rotation.

### 4. Refresh only the access token
```bash
curl -X POST http://localhost:5000/api/auth/refresh-access \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"<refresh-token-from-login>"}'
```

This endpoint returns a fresh access token while keeping the current refresh token valid.

### 5. Logout
```bash
curl -X POST http://localhost:5000/api/auth/logout \
  -H "Content-Type: application/json" \
  -d '{"refreshToken":"<refresh-token-from-login>"}'
```

### 5. Health check
```bash
curl http://localhost:5000/api/health
```

## Single-session behavior
When a user logs in again, any existing active session for that username is revoked. This ensures only one active session is allowed per username.

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
