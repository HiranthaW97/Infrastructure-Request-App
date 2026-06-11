# Infrastructure Request App

A web application that lets staff report technical and infrastructure issues to the
infrastructure team, and lets that team triage, assign, and resolve them in one place.
Instead of issues being raised over scattered emails or chat messages, every request is
captured with a type, urgency, description, status, and a comment thread — giving both
the reporter and the infra team a single source of truth.

## Features

- **Submit requests** — users log a request with a type (Hardware, Software, Network, Other),
  urgency (Low/Medium/High), description, and optional note.
- **Track your requests** — users can view the status of the requests they have raised.
- **Admin dashboard** — the infrastructure team can view all requests, filter by status,
  open request details, change status, and assign requests to a team member.
- **Comment thread** — back-and-forth notes between the reporter and infra team on each request.
- **User management** — admins can create and manage users.
- **Authentication & roles** — cookie-based sign-in with `Admin` and standard user roles,
  plus password change / forced reset.

## Tech stack

| Area      | Technology                                  |
|-----------|---------------------------------------------|
| Framework | .NET 8, ASP.NET Core Blazor Server (Interactive Server) |
| UI        | Radzen Blazor components                     |
| Data      | Entity Framework Core 9 + SQL Server         |
| Auth      | Cookie authentication, role-based authorization |

## Project structure

```
InfrastructureRequestApp/
├─ Components/
│  ├─ Pages/
│  │  ├─ Admin/      # Dashboard, ManageUsers, RequestDetails
│  │  ├─ Auth/       # Login, Logout, ChangePassword
│  │  └─ Requests/   # NewRequest, MyRequests
│  └─ Layout/        # MainLayout, NavMenu
├─ Data/
│  ├─ Entities/      # Request, RequestComment, User
│  ├─ Services/      # AuthService, RequestService, UserService, PasswordHasher
│  └─ InfraRequestsDbContext.cs
├─ Security/         # CustomAuthStateProvider, Roles
└─ Program.cs
```

## Getting started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full)

### Configure the database connection

Do **not** put credentials in `appsettings.json`. Use [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) for local development:

```bash
cd InfrastructureRequestApp
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=YOUR_SERVER;Database=InfraRequestsDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
```

### Run

```bash
dotnet restore
dotnet run --project InfrastructureRequestApp
```

The app starts on the URL shown in the console (see `Properties/launchSettings.json`).

## Roadmap / planned improvements

- [ ] Move all secrets out of source control; rotate any exposed credentials.
- [ ] Replace plain SHA-256 hashing with a salted algorithm (ASP.NET Core `PasswordHasher` / BCrypt / Argon2).
- [ ] Issue a real auth cookie on sign-in so sessions survive refresh/reconnect.
- [ ] Add EF Core migrations for reproducible schema setup.
- [ ] Add server-side validation (`[Required]`, length limits) on request and user forms.
- [ ] Paginate the admin dashboard and add search/filtering.
- [ ] Notify the infrastructure team (email/Teams) when a new request is submitted.
- [ ] Add login rate limiting / account lockout.
- [ ] Add automated tests and structured logging.

## License

Internal project — add a license before distributing.
