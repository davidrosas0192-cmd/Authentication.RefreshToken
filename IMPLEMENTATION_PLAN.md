# Implementation Plan

## Goals
- Build a RESTful .NET 10 API with controllers.
- Use EF Core with a real SQL Server database, not an in-memory store.
- Implement authentication with access tokens and refresh tokens.
- Enforce a single active session per username by revoking prior sessions during login and refresh.

## Deliverables
1. Domain model for users and sessions.
2. EF Core DbContext and migration support.
3. Auth endpoints for registration, login, refresh, and logout.
4. JWT access token generation and refresh token rotation.
5. Documentation for setup and usage.

## Implementation Notes
- The API uses SQL Server via connection string configuration.
- Passwords are hashed with BCrypt.
- Refresh tokens are stored as hashes and rotated on each refresh.
- Active sessions are revoked when a new login or refresh occurs for the same user.

## Local Run Checklist
- Start SQL Server.
- Apply migrations with `dotnet ef database update`.
- Run the API with `dotnet run`.
- Use the auth endpoints to create a user, log in, refresh, and log out.
