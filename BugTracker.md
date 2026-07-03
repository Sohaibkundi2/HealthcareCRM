# Healthcare CRM — Bug Tracker

| # | Date | Feature | Bug Description | Steps to Reproduce | Severity | Status | Fixed In |
|---|------|---------|----------------|-------------------|----------|--------|----------|
| 1 | 2026-07-04 | Patient Module | Save button shows empty error on missing required fields | 1. Click Add Patient 2. Leave name empty 3. Click Save | Medium | Fixed | feat/patient-module |
| 2 | 2026-07-04 | Auth | HomeController redirecting to login after successful login | 1. Login with valid credentials 2. Gets redirected back to login | High | Fixed | fix/home-controller-auth |

## Severity Levels
- **Critical** — app crashes or data loss
- **High** — feature broken, no workaround
- **Medium** — feature partially broken
- **Low** — minor UI/UX issue

## Status Options
- **Open** — not yet fixed
- **In Progress** — being worked on
- **Fixed** — resolved
- **Closed** — verified and closed