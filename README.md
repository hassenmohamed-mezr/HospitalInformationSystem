# Hospital Information System

## 1. Project title and overview

**Hospital Information System** is an operational web application for managing hospital staff accounts, patient demographics, doctor profiles, clinical visits, and outpatient appointments. It targets day-to-day coordination between **Administration**, **Clinical staff (doctors)**, and **Reception**.

The system provides authenticated access, role-scoped screens, CRUD-style maintenance for core entities, **appointment scheduling with validation and status workflow**, **automatic creation of a completed visit when an appointment is completed**, and **role-specific dashboards** with KPIs and operational shortcuts.

It is built as a traditional **ASP.NET MVC** server-rendered application (not a SPA), with **Razor views**, **Entity Framework Core**, and a **SQLite** database file as shipped in configuration.

---

## 2. System architecture

| Concern | Implementation |
|--------|----------------|
| **Web framework** | ASP.NET Core MVC on **.NET 9** |
| **UI** | Razor Views, Bootstrap 5, shared layout, Bootstrap Icons (CDN) |
| **Data access** | Entity Framework Core (`ApplicationDbContext`) |
| **Database** | **SQLite** (`HospitalDB.db` via `appsettings.json` connection string); EF Core SQL Server package is referenced for optional future use but the running app is configured for SQLite |
| **Authentication** | **Custom** credential check against the `Users` table; **session** stores `UserId`, `Username`, and `Role` after login |
| **Authorization** | **Role-based** checks in controllers (`Admin`, `Doctor`, `Reception`); no ASP.NET Core Identity |
| **Business logic** | Primarily in **MVC controllers** (validation, queries, workflow) |
| **UI separation** | **ViewModels** for forms and complex screens (e.g. patients, users, doctors, visits, appointments); dashboards currently use **ViewBag** for aggregates plus a small **ViewModel** for reusable KPI cards and doctor timeline rows |
| **Database initialization** | `Program.cs` applies EF migrations on startup and runs **seed data** |

Default route: **`Auth` / `Login`**.

---

## 3. Core modules (current implementation)

### Users module (Admin)

- **List / create / edit** staff `User` records (Admin only).
- **Role assignment** (`Admin`, `Doctor`, `Reception`) and **active flag** (`IsActive`); inactive users cannot sign in.
- **Password hashing** on create/update (not stored in plain text).

### Authentication module

- **Login / logout** via `AuthController`.
- **Session-based** post-login state; idle timeout configured in `Program.cs` (distributed memory cache + session middleware).
- **Role redirect** after login to the appropriate dashboard controller.

### Patients module

- **CRUD** for patient records: Admin and Reception may create and edit; Admin, Reception, and Doctor may view the index.
- **Search filters** on the patient list: optional **full name**, **national ID**, and **phone number** (substring match where applicable).
- **Status management**: `IsActive` on create/edit; unique **National ID** when provided (application + database unique index).

### Doctors module (Admin)

- **CRUD** for `DoctorProfile`, each **linked 1:1** to a `User` who acts as the doctor account.
- Fields include **specialty**, **room**, **years of experience**, and profile/user active flags as modeled in the app.
- **Search**: dedicated search action filtering by **doctor name** (from linked user) and **specialty**.

### Visits module

- **Medical visit records** with **patient**, **doctor profile**, **visit date/time**, optional **reason** and **notes**, and **status** (`Pending`, `InProgress`, `Completed`, `Cancelled`).
- **Reception** creates visits; **doctors** edit **notes** and **status** for their own visits only; **Admin** and **Reception** have broader visibility; **Details** enforces doctor ownership where applicable.
- Visits are **independent** of appointments except when an appointment is **completed** (see Appointments), which **inserts** a new visit row.

### Appointment module

- **List** (`Index`) with optional filters: **status**, **date from**, **date to**. Doctors see **only their** appointments; Admin and Reception see all.
- **Create** and **Edit** (Admin and Reception only for those actions in the controller).
- **Details** for all roles that may view appointments; doctors may only open their own rows.
- **Workflow HTTP POST actions** (from the appointment UI):
  - **Confirm**: allowed for Doctor, Reception, or Admin; only from **`Pending`** → **`Confirmed`** (doctor limited to own appointments).
  - **Cancel**: sets status to **`Cancelled`** (same role rules as confirm for doctors).
  - **Complete**: only from **`Confirmed`** → **`Completed`**; on success creates a **`Visit`** with **completed** status, same patient and doctor, visit time aligned to the appointment, and generated reason/notes text derived from the appointment.
- **Business rules (enforced in `AppointmentsController`)**:
  - **Future start**: `AppointmentDateTime` must be **after** `DateTime.Now` on create and edit.
  - **Working hours**: appointment **start** must satisfy `dateTime.Hour >= 9 && dateTime.Hour < 17` (roughly **9:00 AM through 4:59 PM** on the start timestamp’s clock hour).
  - **Overlap prevention** for the same doctor: new or moved slots cannot overlap existing appointments **excluding cancelled** ones; duration defaults to **30 minutes** when not specified, and overlap uses each row’s stored duration.
- **Admin/Reception edit** can update scheduling fields and **status** via the edit form in addition to the dedicated workflow actions (administrative correction path).
- **No dedicated “delete appointment”** controller action: cancellation is the lifecycle path for retiring a slot.

---

## 4. Dashboard system

Shared patterns: **page header** (title, role context, date), **KPI cards** via **`Views/Shared/_KpiCard.cshtml`** and `KpiCardViewModel`, **card-based** insights, **quick actions**, and optional **tables / timelines**. Styling relies on **Bootstrap 5** utilities and **centralized CSS** in `wwwroot/css/site.css` (dashboard section).

### Admin dashboard (`AdminController` / `Views/Admin/Index.cshtml`)

- **KPIs**: total users, active users, total patients, total doctors, visits today, total appointments (with pending count as context), **combined “system activity today”** metric (visits today + new patients today + appointments scheduled today), plus **active user percentage** for trend text.
- **Insights**: latest staff users (table), recent patient registrations, short **visit pulse** list (recent visits in a near-term window).
- **Quick actions**: manage users, manage doctors; **reports** and **system settings** are **placeholders** (disabled controls, not implemented modules).

### Doctor dashboard (`DoctorController` / `Views/Doctor/Index.cshtml`)

- **KPIs**: total appointments, today’s appointments, pending appointments, completed visits (lifetime for that doctor).
- **Next appointment** spotlight when the next future non-cancelled appointment exists.
- **Timeline**: **today’s** **appointments and visits merged** and sorted by time (`DoctorScheduleLineViewModel` list).
- **Recently treated**: latest **completed** visits with quick link to edit notes.
- **Quick actions**: appointments list, visits list, patient directory.

### Reception dashboard (`ReceptionController` / `Views/Reception/Index.cshtml`)

- **KPIs**: total patients, new patients today, appointments today (with confirmed count), pending appointments **today**, completed appointments **today**; active patient count as supporting text where provided.
- **Insights**: **today’s appointment queue** (time-ordered table, cancelled excluded in query), **recently registered** patients.
- **Quick actions**: create appointment, register patient, view visits, manage appointments.

---

## 5. UI and UX system

- **Bootstrap 5** for layout, grid, components, and utilities.
- **Bootstrap Icons** loaded from CDN in `_Layout.cshtml`.
- **Reusable KPI partial** `_KpiCard.cshtml` for consistent metric tiles (icon, title, value, optional trend line).
- **`site.css`**: design tokens, shared components (`app-card`, tables, login), and **dashboard-specific** classes (hero header, timeline, spotlight) without inline CSS in dashboard views.
- **`_Layout.cshtml`**: navigation varies by **session role** (dashboard links, patients, appointments, users/doctors for Admin).
- **Status presentation**: Bootstrap **badges** (`text-bg-*`) for appointment and visit states in dashboards and lists.

---

## 6. Data model overview

| Entity | Purpose |
|--------|---------|
| **User** | Staff login identity: name, username, email, password hash, **role**, **IsActive**. |
| **UserProfile** | Optional 1:1 extended profile data for a user (modeled in EF; not all screens may expose it). |
| **DoctorProfile** | Clinical profile: **UserId** (unique), specialty, room, experience, **IsActive**; collections of visits and appointments. |
| **Patient** | Demographics and contact data, **IsActive**, timestamps; optional unique national ID. |
| **Visit** | Encounter linking **PatientId** + **DoctorProfileId**, datetime, reason, notes, **VisitStatus**. |
| **Appointment** | Scheduled slot linking **PatientId** + **DoctorProfileId**, **AppointmentDateTime**, optional **Duration** (minutes), **AppointmentStatus**, notes, timestamps. |

**Relationships (cardinality)**

- **Patient → Visits**: **one-to-many** (`Patient` has many `Visit`; FK on `Visit.PatientId`).
- **DoctorProfile → Visits**: **one-to-many** (`DoctorProfile` has many `Visit`; FK on `Visit.DoctorProfileId`).
- **Patient → Appointments**: **one-to-many** (`Patient` has many `Appointment`; FK on `Appointment.PatientId`).
- **DoctorProfile → Appointments**: **one-to-many** (`DoctorProfile` has many `Appointment`; FK on `Appointment.DoctorProfileId`).
- **User → DoctorProfile**: **one-to-one** (unique FK `DoctorProfile.UserId`).

Delete behaviors for visit and appointment FKs are **Restrict** in EF configuration to avoid accidental cascade data loss.

---

## 7. Business rules summary

| Area | Rule |
|------|------|
| **Login** | Valid credentials and **active** user required; generic error message on failure. |
| **Appointments – time** | Start must be **in the future** at save time; **working-hour** check: `Hour` in **9–16** inclusive (`>= 9` and `< 17`). |
| **Appointments – overlap** | Same **doctor** may not have overlapping non-cancelled appointments; duration participates in the interval test. |
| **Appointments – confirm** | Only from **Pending** to **Confirmed**; doctor only for own rows. |
| **Appointments – complete** | Only from **Confirmed** to **Completed**; creates a **completed Visit** linked to the same patient and doctor. |
| **Appointments – cancel** | Sets **Cancelled** (doctor only for own rows when initiated by doctor). |
| **Visits** | Reception creates; doctors update **their** visits only; listing filtered for doctors. |
| **Patients** | National ID uniqueness; date of birth cannot be in the future; inactive patients excluded from some dropdowns (e.g. visit/appointment creation lists). |

---

## 8. Technology stack

- **ASP.NET Core MVC** (.NET 9)
- **Entity Framework Core 9** (SQLite provider in use)
- **SQLite** database file (configurable via `ConnectionStrings:DefaultConnection`)
- **Razor Views**
- **Bootstrap 5**
- **Bootstrap Icons** (CDN)
- **jQuery** (bundled with the template for validation/scripts where used)
- **Session-based authentication** (custom user store, not Identity)

---

## 9. Project status

- The application is **functional** as a modular MVC solution: authentication, role routing, CRUD for users/patients/doctors/visits, and the **appointment** module with validation, overlap checks, workflow actions, and **visit creation on completion**.
- **Dashboards** for Admin, Doctor, and Reception are **implemented** with KPIs, insights, and quick actions aligned to each role.
- **UI** is **consistent** with shared layout, Bootstrap 5, reusable KPI partials, and centralized CSS; some admin quick actions remain **explicit placeholders** for future modules.

---

## 10. Future improvements

Realistic next steps that are **not** required for the current codebase but would extend it:

- **Reporting and analytics**: operational and clinical summary reports, export (PDF/CSV), date-range analytics beyond dashboard aggregates.
- **Notifications**: email or SMS for appointment reminders and status changes.
- **Rich scheduling UI**: full calendar or resource view per doctor, drag-and-drop rescheduling with the same validation rules server-side.
- **Performance**: caching of dashboard aggregates, read-only queries, and pagination on large lists.
- **Architecture**: strongly typed dashboard query/service layer to replace ViewBag-heavy actions; automated tests for overlap and working-hour rules.
- **API layer**: optional REST API for mobile or external scheduling while reusing validation in shared services.
- **Soft-delete or audit trail** for appointments and visits if regulatory retention is required.

---

## Getting started

**Prerequisites:** [.NET 9 SDK](https://dotnet.microsoft.com/download), a clone of this repository.

**Run locally**

```bash
dotnet restore
dotnet ef database update   # optional if migrations are not already applied; startup also migrates
dotnet run
```

Browse to the site root (default **`/Auth/Login`**). Ensure `appsettings.json` points to the desired SQLite path if you relocate the database file.

**Note:** Do not commit production secrets or personal health data. The SQLite file and WAL/SHM sidecars may appear during local runs; exclude sensitive copies from public repositories as appropriate.
