# Healthcare CRM — Test Cases

## Auth Feature Test Cases (7)
| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | Valid Login | 1. Go to /Account/Login 2. Enter valid email and password 3. Click Login | JWT stored in localStorage, redirected to dashboard | ✅ Pass |
| 2 | Invalid Password | 1. Go to /Account/Login 2. Enter valid email but wrong password 3. Click Login | Error message "Invalid email or password" shown | ✅ Pass |
| 3 | Empty Login Form | 1. Go to /Account/Login 2. Leave all fields empty 3. Click Login | Validation errors shown for each field | ✅ Pass |
| 4 | Valid Registration | 1. Go to /Account/Register 2. Fill all fields correctly 3. Click Register | User saved in database, JWT stored, redirected to dashboard | ✅ Pass |
| 5 | Duplicate Email Registration | 1. Go to /Account/Register 2. Enter already registered email 3. Click Register | Error message "Email already exists" shown | ✅ Pass |
| 6 | Protected Route Without Token | 1. Clear localStorage 2. Go to /Home/Index directly | Redirected to /Account/Login automatically | ✅ Pass |
| 7 | Password Mismatch Registration | 1. Go to /Account/Register 2. Enter different passwords 3. Click Register | Error message "Passwords do not match" shown | ✅ Pass |

## Patient Module Test Cases (10)
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

## Doctor Module Test Cases (7)
| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | View Doctor List | 1. Login 2. Go to /Doctors | All active doctors shown in table | ✅ Pass |
| 2 | Add Valid Doctor | 1. Click + Add Doctor 2. Fill all fields 3. Click Save | Doctor added and appears in list | ✅ Pass |
| 3 | Edit Doctor | 1. Click Edit on a doctor 2. Change details 3. Click Save | Doctor details updated | ✅ Pass |
| 4 | Deactivate Doctor | 1. Click Deactivate 2. Confirm dialog | Doctor disappears from active list | ✅ Pass |
| 5 | Reactivate Doctor | 1. Toggle Show Inactive 2. Click Reactivate 3. Confirm | Doctor reappears in active list | ✅ Pass |
| 6 | Show Inactive Toggle | 1. Toggle Show Inactive switch | Inactive doctors appear in list | ✅ Pass |
| 7 | Add Doctor with Empty Fields | 1. Click + Add Doctor 2. Leave fields empty 3. Click Save | Validation error shown | ✅ Pass |

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