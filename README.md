# DSGNLTest

## Overview
DSGNLTest is a modern System built with .NET 8. It features a decoupled architecture with a secure Web API backend and a responsive Razor Pages frontend. 

## Tech Stack & Requirements
*   **Framework**: .NET 8.0
*   **Backend**: ASP.NET Core Web API
*   **Frontend**: ASP.NET Core Razor Pages
*   **Authentication**: JWT (JSON Web Tokens)
*   **Security**: BCrypt for password hashing
*   **Documentation**: Swagger/OpenAPI
*   **Testing**: xUnit, Moq, Microsoft.AspNetCore.Mvc.Testing

## Configuration & Running the Project

### Prerequisites
*   .NET 8.0 SDK installed.

### 1. Configure the API
Navigate to `src/Designli.Api/appsettings.json` and ensure the JWT settings are configured (default values provided for development):
```json
"Jwt": {
  "Key": "THIS_IS_A_SUPER_SECRET_KEY_123456789",
  "Issuer": "DesignliIssuer",
  "Audience": "DesignliAudience"
}
```

### 2. Configure the Web App
Navigate to `src/Designli.Web/appsettings.json` and verify the API Base URL:
```json
"ApiBaseUrl": "https://localhost:7063"
```

### 3. Run the Application
You need to run both the API and the Web projects.

**Option A: Visual Studio**
1.  Open `DesignliApp.sln`.
2.  Configure Multiple Startup Projects: Right-click Solution -> Properties -> Startup Project -> Select "Multiple startup projects".
3.  Set `Designli.Api` and `Designli.Web` to "Start".
4.  Press F5.

**Option B: CLI**
Open two terminal windows:

Terminal 1 (API):
```bash
cd src/Designli.Api
dotnet run
```

Terminal 2 (Web):
```bash
cd src/Designli.Web
dotnet run
```
*Note: Ensure the API is running on the port expected by the Web app (default `https://localhost:7063`).*

## How it Works
The solution follows a **Clean Architecture** approach:
*   **Designli.Domain**: Contains core entities (`Employee`, `User`) and repository interfaces. No external dependencies.
*   **Designli.Infrastructure**: Implements data access and services (e.g., `EmployeeRepository`, `JwtTokenService`).
*   **Designli.Api**: Exposes RESTful endpoints and handles JWT authentication.
*   **Designli.Web**: A Razor Pages application that consumes the API. It uses `IHttpClientFactory` for typed clients (`AuthApiService`, `EmployeeApiService`) and manages user sessions.

### Authentication Flow
1.  User logs in via the Web UI.
2.  Web app sends credentials to the API.
3.  API validates credentials and issues a JWT.
4.  Web app stores the JWT in a secure HTTP-only cookie/session.
5.  Subsequent requests to the API include the JWT in the Authorization header.

## Testing
The solution includes comprehensive Unit and Integration tests in `src/Designli.Tests`.

To run all tests:
```bash
dotnet test src/Designli.Tests
```

*   **Unit Tests**: Cover Controllers, Services, and Models.
*   **Integration Tests**: Verify the full authentication flow using `WebApplicationFactory`.

## Bonus Features
*   **Dashboard**: A modern landing page displaying key metrics and quick actions.
*   **Employees CRUD**: Full Create, Read, Update, and Delete functionality for employee records.
*   **Logout**: Secure logout functionality that clears the user session and redirects to the login page.