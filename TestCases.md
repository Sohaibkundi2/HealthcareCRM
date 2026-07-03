# Healthcare CRM — Test Cases

## Auth Feature Test Cases

| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | Valid Login | 1. Go to /Account/Login 2. Enter valid email and password 3. Click Login | JWT stored in localStorage, redirected to dashboard | ✅ Pass |
| 2 | Invalid Password | 1. Go to /Account/Login 2. Enter valid email but wrong password 3. Click Login | Error message "Invalid email or password" shown | ✅ Pass |
| 3 | Empty Login Form | 1. Go to /Account/Login 2. Leave all fields empty 3. Click Login | Validation errors shown for each field | ✅ Pass |
| 4 | Valid Registration | 1. Go to /Account/Register 2. Fill all fields correctly 3. Click Register | User saved in database, JWT stored, redirected to dashboard | ✅ Pass |
| 5 | Duplicate Email Registration | 1. Go to /Account/Register 2. Enter already registered email 3. Click Register | Error message "Email already exists" shown | ✅ Pass |
| 6 | Protected Route Without Token | 1. Clear localStorage 2. Go to /Home/Index directly | Redirected to /Account/Login automatically | ✅ Pass |
| 7 | Password Mismatch Registration | 1. Go to /Account/Register 2. Enter different passwords in Password and Confirm Password 3. Click Register | Error message "Passwords do not match" shown | ✅ Pass |