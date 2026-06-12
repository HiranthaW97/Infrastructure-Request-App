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
- **User management** — admins can create and manage users (full name, username, email, role).
- **Authentication & roles** — cookie-based sign-in with `Admin` and standard user roles,
  plus self-service password change.
- **Forgot password** — from the login page a user requests recovery by username; the app
  generates a random temporary password, emails it to the address on the account, and flags
  the account. On the next sign-in the user is forced through a reset page to choose a new
  password before continuing.

## Tech stack

| Area      | Technology                                  |
|-----------|---------------------------------------------|
| Framework | .NET 8, ASP.NET Core Blazor Server (Interactive Server) |
| UI        | Radzen Blazor components                     |
| Data      | Entity Framework Core 9 + SQL Server         |
| Auth      | Cookie authentication, role-based authorization |
| Email     | SMTP via `System.Net.Mail` (Gmail), console fallback in dev |

## Project structure

```
InfrastructureRequestApp/
├─ Components/
│  ├─ Pages/
│  │  ├─ Admin/      # Dashboard, ManageUsers, RequestDetails
│  │  ├─ Auth/       # Login, Logout, ChangePassword, ResetPassword
│  │  └─ Requests/   # NewRequest, MyRequests
│  └─ Layout/        # MainLayout, NavMenu, AuthLayout
├─ Data/             # Persistence only
│  ├─ Entities/      # Request, RequestComment, User
│  ├─ Scripts/       # AddPasswordRecoveryColumns.sql
│  └─ InfraRequestsDbContext.cs
├─ Services/         # Application/business logic (behind interfaces)
│  ├─ Interfaces/    # IAuthService, IUserService, IRequestService, IEmailService
│  ├─ Email/         # EmailSettings, SmtpEmailService
│  ├─ AuthService.cs
│  ├─ UserService.cs
│  ├─ RequestService.cs
│  └─ PasswordHasher.cs
├─ Security/         # CustomAuthStateProvider, Roles
└─ Program.cs        # Composition root (DI + auth + sign-in endpoint)
```

## Getting started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express, or full)

### Configuration & secrets

The committed `appsettings.json` ships with **blank** secrets. Real credentials go in
`appsettings.Development.json`, which is **git-ignored** (so secrets never get committed).
Create it under `InfrastructureRequestApp/` with your local values:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=InfraRequestsDb;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;"
  },
  "Email": {
    "UserName": "you@gmail.com",
    "Password": "your-16-char-gmail-app-password"
  }
}
```

> User secrets (`dotnet user-secrets`) are an equally valid alternative if you prefer to keep
> secrets out of the project folder entirely.

### Email (password recovery)

Password-recovery emails are sent over SMTP. Defaults (`smtp.gmail.com:587`, SSL) live in the
`Email` section of `appsettings.json`; only `UserName` / `Password` need to be supplied.

- For Gmail, `Password` must be a **16-character App Password** (Google Account → Security →
  2-Step Verification → App passwords), **not** your account password.
- If no SMTP credentials are configured, `SmtpEmailService` falls back to **logging** the email
  (including the temporary password) to the console — so the whole flow is testable in
  development without real credentials.

### Database setup

The schema is database-first (scaffolded with EF Core Power Tools — no migrations). Apply the
one-off script that adds the password-recovery columns (`Email`, `MustResetPassword`) to the
`Users` table:

```
InfrastructureRequestApp/Data/Scripts/AddPasswordRecoveryColumns.sql
```

Run it once against `InfraRequestsDb` in SSMS or via `sqlcmd`. It is idempotent (guarded with
`IF NOT EXISTS`), so re-running is safe.

### Run

```bash
dotnet restore
dotnet run --project InfrastructureRequestApp
```

The app starts on the URL shown in the console (see `Properties/launchSettings.json`).

## Roadmap / planned improvements

- [x] Keep secrets out of the committed config (blank `appsettings.json` + git-ignored `appsettings.Development.json`).
- [ ] Rotate the DB password that was exposed in earlier commit history; scrub history if the repo is shared.
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
