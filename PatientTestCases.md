# Patient Module — Test Cases

| # | Test Case | Steps | Expected Result | Status |
|---|-----------|-------|----------------|--------|
| 1 | View Patient List | 1. Login 2. Go to /Patients | All patients shown in table | ✅ Pass |
| 2 | Search Patient | 1. Go to /Patients 2. Type name in search bar | Filtered patients shown instantly | ✅ Pass |
| 3 | Add New Patient | 1. Click + Add Patient 2. Fill all fields 3. Click Save | Patient added and appears in list | ✅ Pass |
| 4 | Edit Patient | 1. Click Edit on a patient 2. Change details 3. Click Save | Patient details updated in list | ✅ Pass |
| 5 | Add Patient with Empty Fields | 1. Click + Add Patient 2. Leave fields empty 3. Click Save | Error shown, patient not saved | ✅ Pass |