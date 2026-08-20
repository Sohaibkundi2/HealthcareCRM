# HealthcareCRM — Handoff Document

## Architecture Overview


## Database Schema

### Users
| Column | Type | Notes |
|--------|------|-------|
| Id | INT | Primary Key |
| FullName | NVARCHAR | Required |
| Email | NVARCHAR | Unique |
| PasswordHash | NVARCHAR | BCrypt hashed |
| Role | NVARCHAR | Admin or Staff |
| IsActive | BIT | Soft disable |
| CreatedAt | DATETIME | Auto |

### Patients
| Column | Type | Notes |
|--------|------|-------|
| Id | INT | Primary Key |
| FullName | NVARCHAR | Required |
| Email | NVARCHAR | Optional |
| Phone | NVARCHAR | Required |
| DateOfBirth | DATETIME | Required |
| Gender | NVARCHAR | Required |
| Address | NVARCHAR | Optional |
| CreatedBy | INT | FK → Users |
| CreatedAt | DATETIME | Auto |

### Doctors
| Column | Type | Notes |
|--------|------|-------|
| Id | INT | Primary Key |
| Name | NVARCHAR | Required |
| Specialization | NVARCHAR | Required |
| Phone | NVARCHAR | Required |
| IsActive | BIT | Soft delete |
| CreatedAt | DATETIME | Auto |

### Appointments
| Column | Type | Notes |
|--------|------|-------|
| Id | INT | Primary Key |
| PatientId | INT | FK → Patients |
| DoctorId | INT | FK → Doctors |
| AppointmentDate | DATETIME | Required |
| Status | NVARCHAR | Pending/Confirmed/Cancelled |
| Notes | NVARCHAR | Optional |
| CreatedAt | DATETIME | Auto |

### AuditLogs
| Column | Type | Notes |
|--------|------|-------|
| Id | INT | Primary Key |
| Action | NVARCHAR | e.g. ROLE_CHANGE |
| TargetType | NVARCHAR | e.g. User |
| TargetId | INT | Target record ID |
| PerformedBy | NVARCHAR | Admin email |
| Details | NVARCHAR | Full description |
| CreatedAt | DATETIME | Auto |

## Setup Guide

See README.md for full setup instructions.

## Demo Accounts

| Role | Email | Password |
|------|-------|----------|
| Admin | Register at /Account/Register → select Admin | Your choice |
| Staff | Register at /Account/Register → select Staff | Your choice |

## Known Issues / Technical Debt

| # | Issue | Priority | Notes |
|---|-------|----------|-------|
| 1 | JWT not validated server-side on every request | High | Deferred to post-internship |
| 2 | No refresh token implemented | Medium | JWT expires after 7 days |
| 3 | No unit tests — manual test cases only | Medium | 65+ manual cases passing |
| 4 | Static files not loading from Visual Studio F5 | Low | Use `dotnet run` instead |
| 5 | No email notifications for appointments | Low | Future feature |

## Future Features Backlog

| # | Feature | Priority |
|---|---------|----------|
| 1 | Email/SMS appointment reminders | High |
| 2 | Patient medical history / prescriptions | High |
| 3 | Doctor availability calendar | High |
| 4 | SignalR real-time appointment notifications | Medium |
| 5 | Multi-clinic support | Medium |
| 6 | Mobile app (React Native) | Medium |
| 7 | Insurance / billing module | Low |
| 8 | Telemedicine / video consultation | Low |