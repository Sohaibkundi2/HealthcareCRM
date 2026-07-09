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
| 10 | View Patient Detail | 1. Click View on a patient | Full patient profile shown on detail page | ✅ Pass |

## Doctor Module Test Cases (5)

| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | View Doctor List | 1. Login 2. Go to /Doctors | All active doctors shown in table | ✅ Pass |
| 2 | Empty Doctor List | 1. Remove all doctors from DB 2. Go to /Doctors | "No doctors found" empty state shown | ✅ Pass |
| 3 | GET /api/doctors returns only active | 1. Set IsActive=false on a doctor in DB 2. Call GET /api/doctors | Inactive doctor not returned | ✅ Pass |
| 4 | Doctor list shows correct status badge | 1. Go to /Doctors | Active doctors show green badge, inactive show grey | ✅ Pass |
| 5 | API returns correct response shape | 1. Call GET /api/doctors in Postman | Response has { success: true, data: [...] } shape | ✅ Pass |