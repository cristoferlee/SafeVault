# SafeVault

SafeVault is an ASP.NET Core web application created to demonstrate secure coding, authentication, authorization, and security testing practices with assistance from Microsoft Copilot.

## Project objectives

The project was developed through three security activities:

1. Secure input handling and SQL injection prevention.
2. Authentication and role-based authorization.
3. Security debugging and attack simulation.

## Security features

### Input validation

- Server-side validation for usernames and email addresses.
- Username allowlist validation.
- Password length validation.
- HTML output encoding for user-generated content.

### SQL injection prevention

Database operations use parameterized queries with `SqlParameter`. User-provided values are never concatenated directly into SQL commands.

### Authentication

- Passwords are hashed using `BCrypt.Net-Next`.
- Plain-text passwords are never stored.
- Login credentials are verified against stored password hashes.
- Cookie-based authentication maintains authenticated sessions.

### Authorization

SafeVault implements Role-Based Access Control with the following roles:

- `User`
- `Admin`

The `AdminOnly` policy restricts access to administrative endpoints.

### XSS prevention

User-generated values are validated and HTML-encoded before being rendered in a response.

## Testing

The project includes NUnit unit and integration tests covering:

- Valid and invalid input.
- SQL injection payloads.
- XSS payloads.
- Valid and invalid login attempts.
- Unknown users.
- Admin and regular user authorization.
- Malicious requests sent through real HTTP endpoints.

Run the tests with:

```powershell
dotnet test

Current result:

Total: 16
Passed: 16
Failed: 0
Technologies
.NET 10
ASP.NET Core Minimal API
SQL Server
Microsoft.Data.SqlClient
BCrypt.Net-Next
NUnit
Microsoft.AspNetCore.Mvc.Testing
Microsoft Copilot
Project structure
SafeVault
├── SafeVault.Web
│   ├── Data
│   ├── Helpers
│   ├── Models
│   ├── Services
│   ├── wwwroot
│   ├── database.sql
│   └── Program.cs
├── SafeVault.Tests
├── SECURITY_SUMMARY.md
└── SafeVault.slnx
Run locally

Restore and build the solution:

dotnet restore
dotnet build

Execute SafeVault.Web/database.sql in SQL Server to create the database and Users table.

Run the application:

dotnet run --project .\SafeVault.Web

Use the URL displayed in the terminal to open the application.

Microsoft Copilot usage

Microsoft Copilot assisted with:

Generating secure input-validation suggestions.
Creating parameterized database queries.
Implementing authentication and RBAC.
Reviewing the codebase for SQL injection and XSS risks.
Generating security test scenarios.
Identifying missing validation and insufficient endpoint-level tests.

All generated suggestions were reviewed, corrected when necessary, and verified through automated tests.

Security review

See SECURITY_SUMMARY.md for the vulnerabilities identified, corrections applied, and final testing results.


Guarda y ejecuta:

```powershell
git add .
git commit -m "Add documentation and remove build artifacts"