# HealthcareCRM

A full-stack Healthcare CRM built with **ASP.NET Core MVC**, **SQL Server**, and **Entity Framework Core** as part of the Friendsware Solutions Summer Internship 2026.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core MVC (.NET 10) |
| Database | SQL Server + Entity Framework Core |
| Authentication | JWT (JSON Web Tokens) |
| Frontend | Razor Views + Bootstrap 5 + Chart.js |
| Documentation | Swagger (OpenAPI) |
| Export | CsvHelper + iText7 (PDF) |

---

## Features

- **Auth & RBAC** — JWT login/register, Admin and Staff roles, protected routes
- **Patient Management** — Full CRUD, search by name/phone/DOB, pagination, CSV export
- **Doctor Management** — Add, edit, soft deactivate/reactivate
- **Appointments** — Book, list, filter by status, update, cancel, PDF report
- **Analytics Dashboard** — Live stats, Chart.js doughnut chart, recent appointments
- **Admin Panel** — User management, role assignment, activate/deactivate, audit log
- **Global Error Handler** — Structured JSON error responses
- **Access Denied Page** — 403 redirect with proper UI

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [SQL Server Management Studio](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)

---

## Setup & Run

**1. Clone the repository**
```bash
git clone https://github.com/Sohaibkundi2/HealthcareCRM.git
cd HealthcareCRM
```

**2. Configure database**

Open `appsettings.json` and update the connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=HealthcareCRM;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**3. Run migrations**
```bash
dotnet ef database update
```

**4. Run the project**
```bash
dotnet run
```

**5. Open in browser**
http://localhost:5240


**6. Access Swagger**

http://localhost:5240/swagger

---

## Default Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | Register via /Account/Register (select Admin role) | Your choice |
| Staff | Register via /Account/Register (select Staff role) | Your choice |


---

## API Endpoints

### Auth
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | /api/auth/login | Login user | None |
| POST | /api/auth/register | Register user | None |
| POST | /api/auth/logout | Logout user | None |

### Patients
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | /api/patients | Get all patients (paginated) | Any |
| GET | /api/patients/{id} | Get patient by ID | Any |
| POST | /api/patients | Create patient | Any |
| PUT | /api/patients/{id} | Update patient | Any |
| DELETE | /api/patients/{id} | Delete patient | Admin |
| GET | /api/patients/export?format=csv | Export CSV | Admin |

### Doctors
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | /api/doctors | Get active doctors | Any |
| GET | /api/doctors/{id} | Get doctor by ID | Any |
| POST | /api/doctors | Create doctor | Any |
| PUT | /api/doctors/{id} | Update doctor | Any |
| PUT | /api/doctors/{id}/deactivate | Deactivate doctor | Any |
| PUT | /api/doctors/{id}/reactivate | Reactivate doctor | Any |

### Appointments
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | /api/appointments | Get all appointments | Any |
| GET | /api/appointments/{id} | Get by ID | Any |
| POST | /api/appointments | Book appointment | Any |
| PUT | /api/appointments/{id}/status | Update status | Any |
| PUT | /api/appointments/{id}/cancel | Cancel | Any |
| GET | /api/appointments/report | PDF report | Admin |

### Dashboard
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | /api/dashboard/stats | Get live stats | Any |

### Admin
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | /api/admin/users | List all users | Admin |
| PUT | /api/admin/users/{id} | Change role | Admin |
| PUT | /api/admin/users/{id}/toggle-active | Toggle active | Admin |
| GET | /api/admin/audit-log | View audit log | Admin |

### Health
| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | /api/health | Health check | None |

---

## Test Cases

68 manual test cases covering:
- Auth (10) — login, register, RBAC
- Patients (12) — CRUD, search, pagination
- Doctors (8) — CRUD, soft delete
- Appointments (10) — booking, status, cancel
- Dashboard (5) — stats, chart
- RBAC (10) — role enforcement
- Export (5) — CSV, PDF
- User Management (8) — activate, role change, audit log

See [TestCases.md](TestCases.md) for full details.

---
