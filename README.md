# 🏥 Hospital Information System (MVP)

A web-based Hospital Management System built with **ASP.NET MVC**.

This project delivers the core operational features (MVP) with a clean, extensible architecture suitable for future expansion.

---

## 📌 Overview

The system manages essential hospital workflows, including:

- User access control (Admin, Doctor, Reception)
- Doctor profile management
- Patient record management (non-medical data)
- Appointment scheduling and tracking

The architecture follows strict separation of concerns, ensuring that user identity, profiles, and domain entities remain decoupled and maintainable.

---

## 🎯 MVP Scope

### 🔐 1. Authentication & Authorization

- Secure Login / Logout functionality
- Role-based access control (Admin, Doctor, Reception)
- User account management

### 👨‍⚕️ 2. Doctor Management

- Full CRUD operations for doctor profiles
- Search and filtering by name or specialty
- Prepared for future scheduling/availability modules

### 🧑‍🤝‍🧑 3. Patient Management

- Full CRUD operations for Patient profiles
- Register and update patient records
- Search and list patients
- Manage demographic and contact information

### 🧾 4. Reception Management

- Full CRUD operations for Reception profiles
- View and manage daily appointment calendar
- Register new patients and update existing demographic records
- Book, reschedule, or cancel appointments on behalf of patients
- Quick search for patients by name, phone, or ID
- Check-in patients for scheduled appointments
- View doctor availability overview (read-only)
- Role-based access: Reception cannot modify medical data or doctor profiles

### 📅 5. Appointment System

- Book, edit, and cancel appointments
- Daily appointment calendar view
- Track appointment status:
  - `Scheduled`
  - `Completed`
  - `Cancelled`

### 🩺 6. Clinical Visits (Basic EMR)

- Record doctor visits
- Diagnosis and treatment notes
- Prescription notes
- Patient visit history

### 🧩 Support Features

- Role-based dashboards
- Input validation and business rule enforcement

---

## 🧠 Design Principles

### Separation of Concerns
Users (authentication) are separated from Profiles (personal data) and Patients (domain entities).

### Extensibility
New modules (e.g., Billing, Lab) can be added without restructuring core logic.

### Modularity
Each module is isolated, improving maintainability and testability.

---

## 🧩 High-Level Architecture

| Layer            | Components                                   |
| ---------------- | -------------------------------------------- |
| Identity Layer   | Users, Roles (ASP.NET Core Identity)         |
| Profile Layer    | UserProfile, DoctorProfile, ReceptionProfile |
| Domain Layer     | Patients, Appointments                       |
| Clinical Layer   | Visits, Prescriptions (Basic)                |
| Future Layer     | Lab, Billing, Insurance, Radiology, Pharmacy |

---

## ⚙️ Tech Stack

| Area       | Technology                        |
| ---------- | --------------------------------- |
| Framework  | ASP.NET MVC (.NET 6 / .NET 8)     |
| ORM        | Entity Framework Core             |
| Database   | SQL Server (Prod) / SQLite (Dev)  |
| Frontend   | Bootstrap 5, jQuery               |

---

## 🚀 Getting Started

### 📋 Prerequisites

- .NET SDK (6.0 or later)
- Visual Studio 2022 or VS Code
- SQL Server LocalDB or SQLite

### ⚙️ Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/YOUR_USERNAME/HospitalInformationSystem.git

2. **Navigate to the project directory**
   ```bash
   cd HospitalInformationSystem
3. **Apply database migrations**
- Using Package Manager Console:
   ```bash
   Update-Database
- Using .NET CLI:
  ```bash
  dotnet ef database update
4. **Run the application**
   ```bash
   dotnet run
- Or press F5 in Visual Studio.

---
  
🔒 Business Rules (Key Constraints)
- Only active user accounts can log in.
- Role-based authorization is enforced across all controllers.
- Doctor schedules prevent overlapping appointments.
- Patient records are independent from system user accounts.
- All required fields are validated before saving to the database.

---

📈 Future Enhancements
- **Full EMR Integration** – Visits, diagnosis history, and e-Prescriptions.
- **Patient Portal** – Self-service appointment booking and record access.
- **Notifications** – Email and SMS reminders.
- **Reporting & Analytics** – Advanced dashboards for hospital management.
- **AI Assistant** – Smart scheduling and workflow optimization.

---

📌 Notes
- Database runtime files (`*.mdf`, `*.ldf`) are excluded via `.gitignore`.
- Ensure `appsettings.json` is properly configured for your environment.
- Avoid committing sensitive data (e.g., connection strings).

---

👨‍💻 Purpose
This is an academic project built with real-world engineering considerations, focusing on:
- Scalable multi-layer architecture
- Clear domain separation
- Practical feature prioritization
- Realistic hospital workflow modeling
