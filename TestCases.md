# Healthcare CRM — Test Cases

## Auth Feature Test Cases (10)
| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | Valid Login | 1. Go to /Account/Login 2. Enter valid email and password 3. Click Login | JWT stored in localStorage, redirected to dashboard | ✅ Pass |
| 2 | Invalid Password | 1. Go to /Account/Login 2. Enter valid email but wrong password 3. Click Login | Error message "Invalid email or password" shown | ✅ Pass |
| 3 | Empty Login Form | 1. Go to /Account/Login 2. Leave all fields empty 3. Click Login | Validation errors shown for each field | ✅ Pass |
| 4 | Valid Registration | 1. Go to /Account/Register 2. Fill all fields correctly 3. Click Register | User saved in database, JWT stored, redirected to dashboard | ✅ Pass |
| 5 | Duplicate Email Registration | 1. Go to /Account/Register 2. Enter already registered email 3. Click Register | Error message "Email already exists" shown | ✅ Pass |
| 6 | Protected Route Without Token | 1. Clear localStorage 2. Go to /Home/Index directly | Redirected to /Account/Login automatically | ✅ Pass |
| 7 | Password Mismatch Registration | 1. Go to /Account/Register 2. Enter different passwords 3. Click Register | Error message "Passwords do not match" shown | ✅ Pass |
| 8 | Login with empty email | 1. Go to /Account/Login 2. Leave email empty 3. Click Login | Validation error "Email is required" shown | ✅ Pass |
| 9 | Register with short password | 1. Go to /Account/Register 2. Enter password less than 6 chars 3. Click Register | Error "Password must be at least 6 characters" | ✅ Pass |
| 10 | Register as Admin role | 1. Go to /Account/Register 2. Select Admin role 3. Register | User created with Admin role, Admin badge shown in navbar | ✅ Pass |

## Patient Module Test Cases (12)
| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | View Patient List | 1. Login 2. Go to /Patients | All patients shown in paginated table | ✅ Pass |
| 2 | Search by Name | 1. Go to /Patients 2. Type patient name in search bar | Filtered patients shown instantly | ✅ Pass |
| 3 | Search by Phone | 1. Go to /Patients 2. Type phone number in search bar | Matching patients shown | ✅ Pass |
| 4 | Search by Date of Birth | 1. Go to /Patients 2. Type date like 1990-05-01 | Matching patients shown | ✅ Pass |
| 5 | Add Valid Patient | 1. Click + Add Patient 2. Fill all required fields 3. Click Save | Patient added and appears in list | ✅ Pass |
| 6 | Add Patient with Empty Fields | 1. Click + Add Patient 2. Leave required fields empty 3. Click Save | Error shown, patient not saved | ✅ Pass |
| 7 | Add Patient with Future DOB | 1. Click + Add Patient 2. Enter future date of birth 3. Click Save | Error "Date of birth cannot be in the future" | ✅ Pass |
| 8 | Edit Patient | 1. Click Edit on a patient 2. Change details 3. Click Save | Patient details updated in list | ✅ Pass |
| 9 | Delete Patient | 1. Click Delete on a patient 2. Confirm in dialog | Patient removed from list | ✅ Pass |
| 10 | View Patient Detail | 1. Click View on a patient | Full patient profile shown with age calculation | ✅ Pass |
| 11 | Search with no results | 1. Go to /Patients 2. Search for non-existent name | "No patients found" empty state shown | ✅ Pass |
| 12 | Pagination works correctly | 1. Add 21+ patients 2. Go to /Patients | Second page shows remaining patients | ✅ Pass |

## Doctor Module Test Cases (8)
| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | View Doctor List | 1. Login 2. Go to /Doctors | All active doctors shown in table | ✅ Pass |
| 2 | Add Valid Doctor | 1. Click + Add Doctor 2. Fill all fields 3. Click Save | Doctor added and appears in list | ✅ Pass |
| 3 | Edit Doctor | 1. Click Edit on a doctor 2. Change details 3. Click Save | Doctor details updated | ✅ Pass |
| 4 | Deactivate Doctor | 1. Click Deactivate 2. Confirm dialog | Doctor disappears from active list | ✅ Pass |
| 5 | Reactivate Doctor | 1. Toggle Show Inactive 2. Click Reactivate 3. Confirm | Doctor reappears in active list | ✅ Pass |
| 6 | Show Inactive Toggle | 1. Toggle Show Inactive switch | Inactive doctors appear in list | ✅ Pass |
| 7 | Add Doctor with Empty Fields | 1. Click + Add Doctor 2. Leave fields empty 3. Click Save | Validation error shown | ✅ Pass |
| 8 | Add Doctor with empty fields | 1. Click + Add Doctor 2. Leave all fields empty 3. Click Save | Validation errors shown for each field | ✅ Pass |

## Appointment Module Test Cases (10)
| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | View Appointment List | 1. Login 2. Go to /Appointments | All appointments shown with status badges | ✅ Pass |
| 2 | Filter by Pending | 1. Click Pending filter | Only pending appointments shown | ✅ Pass |
| 3 | Filter by Confirmed | 1. Click Confirmed filter | Only confirmed appointments shown | ✅ Pass |
| 4 | Filter by Cancelled | 1. Click Cancelled filter | Only cancelled appointments shown | ✅ Pass |
| 5 | Book Valid Appointment | 1. Go to /Appointments/Book 2. Select patient, doctor, date 3. Click Book | Appointment created with Pending status | ✅ Pass |
| 6 | Book with Past Date | 1. Go to /Appointments/Book 2. Select past date 3. Click Book | Error "Appointment date cannot be in the past" | ✅ Pass |
| 7 | Book with Empty Fields | 1. Go to /Appointments/Book 2. Leave fields empty 3. Click Book | Validation errors shown | ✅ Pass |
| 8 | Update Appointment Status | 1. Click Update on appointment 2. Select Confirmed 3. Click Update | Status badge updates to Confirmed | ✅ Pass |
| 9 | Cancel Appointment | 1. Click Cancel on appointment 2. Confirm dialog | Status updates to Cancelled, Cancel button hidden | ✅ Pass |
| 10 | Dashboard Stats | 1. Go to /Home/Index | Shows correct counts for patients, doctors, pending and confirmed appointments | ✅ Pass |

## Dashboard Stats Test Cases (5)

| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | Stats load on dashboard | 1. Login 2. Go to /Home/Index | All metric cards show correct numbers from database | ✅ Pass |
| 2 | Stats with zero records | 1. Empty database 2. Go to /Home/Index | All cards show 0, chart shows empty state | ✅ Pass |
| 3 | Chart renders correctly | 1. Login 2. Go to /Home/Index | Doughnut chart shows Pending, Confirmed, Cancelled breakdown | ✅ Pass |
| 4 | Appointments today count | 1. Book appointment for today 2. Go to /Home/Index | Today's count increments by 1 | ✅ Pass |
| 5 | Stats endpoint unauthorized | 1. Call GET /api/dashboard/stats without token | Returns 401 Unauthorized | ✅ Pass |

## RBAC Test Cases (10)

| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | Admin sees Admin badge | 1. Login as Admin | Orange Admin badge shown in navbar | ✅ Pass |
| 2 | Staff sees Staff badge | 1. Login as Staff | Green Staff badge shown in navbar | ✅ Pass |
| 3 | Admin menu visible for Admin | 1. Login as Admin | ⚙️ Admin link visible in navbar | ✅ Pass |
| 4 | Admin menu hidden for Staff | 1. Login as Staff | ⚙️ Admin link not visible in navbar | ✅ Pass |
| 5 | Staff blocked from Admin page | 1. Login as Staff 2. Go to /Admin/Users directly | Access Denied page shown | ✅ Pass |
| 6 | Admin can access Admin page | 1. Login as Admin 2. Go to /Admin/Users | User list shown successfully | ✅ Pass |
| 7 | Staff blocked from delete patient | 1. Login as Staff 2. Call DELETE /api/patients/1 in Postman | Returns 403 Forbidden | ✅ Pass |
| 8 | Admin can delete patient | 1. Login as Admin 2. Call DELETE /api/patients/1 in Postman with Admin token | Patient deleted successfully | ✅ Pass |
| 9 | Admin can change user role | 1. Login as Admin 2. Go to /Admin/Users 3. Change Staff to Admin | Role updated, badge changes on next login | ✅ Pass |
| 10 | Invalid token blocked | 1. Call GET /api/admin/users with invalid token | Returns 401 Unauthorized | ✅ Pass |

## Export Test Cases (5)

| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | Export patients as CSV (Admin) | 1. Login as Admin 2. Go to /Patients 3. Click Export CSV | CSV file downloads with all patient data | ✅ Pass |
| 2 | Export button hidden for Staff | 1. Login as Staff 2. Go to /Patients | Export CSV button not visible | ✅ Pass |
| 3 | Export CSV via Postman (Admin) | 1. Call GET /api/patients/export?format=csv with Admin token | CSV file returned with correct headers | ✅ Pass |
| 4 | Export CSV blocked for Staff | 1. Call GET /api/patients/export?format=csv with Staff token | Returns 403 Forbidden | ✅ Pass |
| 5 | PDF report via Postman (Admin) | 1. Call GET /api/appointments/report?from=2026-01-01&to=2026-12-31 with Admin token | PDF file downloaded with appointment data | ✅ Pass |

## User Management Test Cases (8)

| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | View all users (Admin) | 1. Login as Admin 2. Go to /Admin/Users | All users shown with role and status | ✅ Pass |
| 2 | Change user role to Admin | 1. Login as Admin 2. Click Change Role 3. Select Admin 4. Update | Role badge updates to Admin | ✅ Pass |
| 3 | Change user role to Staff | 1. Login as Admin 2. Click Change Role 3. Select Staff 4. Update | Role badge updates to Staff | ✅ Pass |
| 4 | Deactivate user | 1. Login as Admin 2. Click Deactivate 3. Confirm | Status badge changes to Inactive | ✅ Pass |
| 5 | Activate user | 1. Login as Admin 2. Click Activate 3. Confirm | Status badge changes to Active | ✅ Pass |
| 6 | Deactivated user cannot login | 1. Deactivate a user 2. Try to login as that user | Error "Your account has been deactivated" | ✅ Pass |
| 7 | Audit log records role change | 1. Change a user role 2. Go to /Admin/AuditLog | ROLE_CHANGE entry shown with details | ✅ Pass |
| 8 | Audit log records deactivation | 1. Deactivate a user 2. Go to /Admin/AuditLog | USER_DEACTIVATED entry shown with details | ✅ Pass |