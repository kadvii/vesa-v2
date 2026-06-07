# Vesa — Iraqi Visa Management System
### Backend Build Prompt (Phase by Phase)

---

## Important Instructions

Build this project **one phase at a time**. After you finish each phase, stop and wait for me to confirm before moving to the next phase. Do not build everything at once.

---

## Style Rules (Follow Exactly — Same as Qi-Salfa)

This project must follow the exact same architecture and coding style as the Qi-Salfa project:

1. **Single-project** ASP.NET Core Web API — no separate `.Application`, `.Domain`, `.Infrastructure` layers
2. **Primary constructors** for all services and controllers:
   ```csharp
   public class ApplicationService(AppDbContext db) : IApplicationService { }
   ```
3. **No Repository / UnitOfWork** — inject `AppDbContext` directly into services
4. **No AutoMapper** — write manual `ToResponse()` private static helper methods inside each service
5. **ASP.NET Core Identity** — use `UserManager<AppUser>` and `SignInManager<AppUser>`. Inline JWT token generation inside `AuthService`
6. **PostgreSQL** via `UseNpgsql()`
7. **Scalar** instead of Swagger UI — `app.MapScalarApiReference()`
8. **Tuple returns** from services: `(bool success, string? error)` or nullable objects
9. **Raw `IActionResult`** in controllers — `Ok()`, `BadRequest(new { error })`, `NotFound()` — no `ApiResponse<T>` wrapper
10. **FluentValidation** — validators in `Validators/` folder
11. **Route conventions:**
    - Admin: `[Route("api/admin/resource")]`
    - Applicant (app): `[Route("api/app/resource")]`
    - Public: `[Route("api/public/resource")]`
12. **EF Core configurations** — `IEntityTypeConfiguration<T>` in `Data/Configurations/`
13. **One class per file**, filename matches class name
14. **Enums** in `Models/Enums/` folder, one file per enum
15. **All DateTime stored as UTC**
16. **`ExceptionHandlingMiddleware`** for global error handling
17. **`IHostedService`** for any background jobs

---

## Project Structure

```
Vesa/
├── Controllers/
├── DTOs/
│   ├── Auth/
│   ├── Applications/
│   ├── Documents/
│   ├── Appointments/
│   ├── Notifications/
│   ├── Payments/
│   ├── VisaTypes/
│   └── Dashboard/
├── Data/
│   ├── AppDbContext.cs
│   ├── Configurations/
│   └── DbSeeder.cs
├── Models/
│   └── Enums/
├── Middlewares/
├── Migrations/
├── Services/
│   ├── Interfaces/
│   └── Background/
├── Validators/
├── Program.cs
└── appsettings.json
```

---

## Domain Overview

**Vesa** is a visa management system for **Iraqi citizens applying for visas to travel abroad**.

- Applicants register, submit visa applications, upload documents, book appointments, and track their application status
- Admins review applications, approve or reject them, request missing documents, manage appointments, and manage visa types per country
- The public can browse available visa types, required documents, fees, and processing times without logging in

---

## Roles

| Role | Description |
|---|---|
| `Applicant` | Iraqi citizen submitting a visa application |
| `Admin` | Visa office staff managing applications |

---

## Phase 1 — Foundation (Build this first, stop when done)

### Entities

**AppUser** (Identity)
- `FullName`, `PhoneNumber`, `NationalId`, `DateOfBirth`, `CreatedAt`

**VisaApplication**
- `Id`, `ApplicantId` (FK → AppUser), `VisaTypeId` (FK), `CountryId` (FK)
- `PassportNumber`, `PassportExpiry`, `TravelDateFrom`, `TravelDateTo`
- `Status` (enum: `Submitted`, `UnderReview`, `ApprovedPending`, `Approved`, `Rejected`, `Cancelled`)
- `AdminNotes`, `RejectionReason`
- `SubmittedAt`, `UpdatedAt`, `ReviewedAt`, `ReviewedByAdminId`

**Country**
- `Id`, `Name`, `IsoCode`, `FlagEmoji`, `IsActive`

**VisaType**
- `Id`, `CountryId` (FK), `Name`, `Description`
- `ProcessingDays`, `FeeAmount`, `IsActive`
- `RequiredDocuments` (list of strings — JSON column)

**Document**
- `Id`, `ApplicationId` (FK), `FileName`, `FileUrl`, `DocumentType` (enum), `UploadedAt`
- `DocumentType` enum: `PassportCopy`, `Photo`, `BankStatement`, `TravelInsurance`, `HotelBooking`, `FlightBooking`, `SponsorLetter`, `Other`

**Notification**
- `Id`, `UserId` (FK), `Title`, `Message`, `IsRead`, `CreatedAt`
- `Type` enum: `ApplicationSubmitted`, `StatusChanged`, `DocumentRequested`, `AppointmentBooked`, `AppointmentReminder`, `General`

### Services to build

- `AuthService` — register (role: Applicant), login, inline JWT
- `VisaApplicationService` — submit, get my applications, get by id, cancel
- `DocumentService` — upload document (save file info), get by application
- `CountryService` — list active countries
- `VisaTypeService` — list by country, get by id
- `NotificationService` — create, get my notifications, mark as read

### Controllers to build

**App (Applicant-facing):**
- `AppAuthController` — register, login
- `AppApplicationsController` — submit, my applications, get by id, cancel
- `AppDocumentsController` — upload, get by application
- `AppNotificationsController` — get mine, mark as read

**Public:**
- `PublicCountriesController` — list countries
- `PublicVisaTypesController` — list by country, get by id (includes required documents and fee)

**Admin:**
- `AdminAuthController` — login only
- `AdminApplicationsController` — get all, get by id, change status (approve/reject/under review), add admin notes, request missing document
- `AdminCountriesController` — create, update, toggle active
- `AdminVisaTypesController` — create, update, toggle active

### Seeding
- Seed 1 admin user: `admin@vesa.iq` / `Admin@12345`
- Seed 5 countries: Iraq neighbors + popular destinations (Turkey, UAE, Germany, UK, Jordan)
- Seed 2-3 visa types per country with realistic fees and processing times
- Seed required documents per visa type

### Config
- PostgreSQL, JWT, CORS for frontend (localhost:3000 and localhost:5173 in dev)
- Auto migrate + seed on startup

---

## Phase 2 — Appointments (Build after Phase 1 is confirmed)

### New Entities

**AppointmentSlot**
- `Id`, `Date`, `Time`, `CountryId` (FK), `MaxCapacity`, `BookedCount`, `IsActive`

**Appointment**
- `Id`, `ApplicationId` (FK), `SlotId` (FK), `ApplicantId` (FK)
- `Status` (enum: `Booked`, `Confirmed`, `Cancelled`, `NoShow`, `Completed`)
- `BookedAt`, `CancelledAt`, `Notes`

### Services to build
- `AppointmentService` — book, cancel, get my appointments
- `AppointmentSlotService` — list available slots, create slot, update slot

### Controllers to build

**App:**
- `AppAppointmentsController` — book appointment (linked to an application), cancel, get mine

**Admin:**
- `AdminAppointmentSlotsController` — create slot, list all slots, toggle active, update capacity
- `AdminAppointmentsController` — list all, confirm, mark no-show, complete

### Business Rules
- One appointment per application
- Cannot book if slot is full (`BookedCount >= MaxCapacity`)
- Cancellation only allowed 24 hours before the appointment time
- Send notification on booking and reminder 24 hours before (via background service)

---

## Phase 3 — Payments (Build after Phase 2 is confirmed)

### New Entities

**Payment**
- `Id`, `ApplicationId` (FK), `ApplicantId` (FK)
- `Amount`, `Currency` (default: `IQD`)
- `Status` (enum: `Pending`, `Paid`, `Failed`, `Refunded`)
- `Method` (enum: `QiCard`, `BankTransfer`, `Cash`)
- `TransactionReference`, `PaidAt`, `CreatedAt`

### Services to build
- `PaymentService` — create payment record, confirm payment, get by application, refund

### Controllers to build

**App:**
- `AppPaymentsController` — get payment for my application, confirm payment

**Admin:**
- `AdminPaymentsController` — list all payments, confirm, refund, filter by status/date

### Business Rules
- Payment is created automatically when an application is submitted (status: `Pending`)
- Application cannot move to `Approved` status until payment is `Paid`
- Admin can manually mark a payment as paid (for cash payments)
- Refund only allowed if application is `Rejected` or `Cancelled`

---

## Phase 4 — Reports & Dashboard (Build after Phase 3 is confirmed)

### No new entities — queries only

### Services to build
- `DashboardService` — overview stats, recent activity

### Controllers to build

**Admin:**
- `AdminDashboardController`:
  - `GET /api/admin/dashboard/overview` — total applications, pending count, approved today, revenue this month, appointments today
  - `GET /api/admin/dashboard/applications-by-status` — count per status
  - `GET /api/admin/dashboard/applications-by-country` — count per country
  - `GET /api/admin/dashboard/revenue` — daily/monthly revenue breakdown
  - `GET /api/admin/dashboard/recent-applications` — last 10 applications with status

---

## Deliverables Per Phase

For each phase, generate files in this order:
1. New models and enums
2. New EF Core configurations
3. New DTOs
4. New service interfaces
5. New service implementations
6. New controllers
7. New validators
8. Updated `AppDbContext` (add new DbSets)
9. Updated `Program.cs` (register new services)
10. Updated `DbSeeder` if needed
11. New migration command to run

After each phase output, write this line:
> ✅ Phase X complete. Confirm to continue to Phase X+1.

---

## Summary Table

| Phase | What gets built |
|---|---|
| Phase 1 | Auth, applications, documents, countries, visa types, notifications |
| Phase 2 | Appointments and slot management |
| Phase 3 | Payments |
| Phase 4 | Admin dashboard and reports |

---

*Vesa — Simple, clean visa management for Iraqi citizens.*
