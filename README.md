# Hospital Information System

![Project Banner](https://img.shields.io/badge/Enterprise-Hospital%20Management-blue?style=for-the-badge&logo=healthcare)

<div align="center">
  <h1>Hospital Information System</h1>
  <p>Enterprise-grade healthcare operations platform with role-based workflows, smart appointment orchestration, and an integrated AI assistant.</p>
  <p>
    <img src="https://img.shields.io/badge/ASP.NET%20Core-9.0-512BD4?style=for-the-badge&logo=dotnet" alt="ASP.NET Core" />
    <img src="https://img.shields.io/badge/FastAPI-Python-009688?style=for-the-badge&logo=fastapi" alt="FastAPI" />
    <img src="https://img.shields.io/badge/AI%20Assistant-RAG-yellowgreen?style=for-the-badge" alt="AI Assistant" />
    <img src="https://img.shields.io/badge/SQLite-Lightgrey?style=for-the-badge&logo=sqlite" alt="SQLite" />
    <img src="https://img.shields.io/badge/EF%20Core-EntityFramework-lightblue?style=for-the-badge&logo=entityframework" alt="EF Core" />
    <img src="https://img.shields.io/badge/Gemini-LLM-ff69b4?style=for-the-badge" alt="Gemini" />
  </p>
</div>

---

## Project Overview

The **Hospital Information System** is a polished, full-stack healthcare operations platform designed for real-world hospital management.

It enables:
- secure role-based access for **Admin**, **Doctors**, and **Reception** teams
- streamlined patient and doctor management
- intelligent appointment scheduling with conflict validation
- visit and clinical record tracking
- embedded AI guidance through a **FastAPI-based RAG assistant**

This project is built as a modern **ASP.NET MVC** application with a dedicated AI microservice, demonstrating strong architectural separation and enterprise workflow support.

---

## Key Features

| Feature | What it delivers |
|---|---|
| ✅ Role-Based Dashboards | Dedicated Admin, Doctor, and Reception dashboards with targeted KPIs and actions. |
| ✅ Session Authentication | Secure login with session-based role control and dynamic navigation. |
| ✅ Patient Management | Create, update, search, and track active patient records. |
| ✅ Doctor Management | Manage doctor profiles, specialties, and linked user accounts. |
| ✅ Appointment Scheduling | Smart scheduling with overlap detection, working hour validation, and lifecycle controls. |
| ✅ Visit Tracking | Create and manage clinical visits, including doctor-owned notes and statuses. |
| ✅ Reception Workflow | Reception-specific KPIs, appointment queue, and patient intake operations. |
| ✅ AI Assistant | Embedded assistant powered by FastAPI, vector retrieval, and Gemini LLM integration. |
| ✅ Responsive UI | Bootstrap-based interface with modern cards, tables, and floating chat. |
| ✅ Search & Filtering | Dynamic filtering for patients, doctors, appointments, and visits. |
| ✅ Dashboard Analytics | Real-time operational metrics for daily hospital performance. |

---

## System Modules

### Admin Module

- Centralized user and doctor management.
- Role-driven dashboards for system oversight.
- Activity metrics for patients, appointments, and visits.
- Business value: governance, operational control, and administrator visibility.

### Doctor Module

- Doctor-specific appointment schedule and visit timeline.
- Summary of pending visits, completed treatments, and next appointment.
- Business value: streamlined physician workflow and patient care focus.

### Reception Module

- Reception dashboard with patient intake metrics.
- Appointment queue, daily progress, and patient registration workflows.
- Business value: front-desk efficiency and same-day appointment coordination.

### AI Assistant Module

- Integrated smart assistant accessible from any authenticated page.
- Provides contextual guidance on system workflows, roles, and processes.
- Powered by a FastAPI service that uses semantic retrieval and Gemini to generate accurate answers.
- Business value: faster onboarding, reduced support load, and contextual system assistance.

---

## AI Assistant

The project includes a professional AI assistant implemented as a dedicated service:

- **FastAPI endpoint** for chat request processing
- **RAG architecture** with semantic retrieval and vector search
- **SentenceTransformers** embeddings + **FAISS** similarity index
- **Gemini LLM** integration for natural language responses
- **Context-aware prompts** built from hospital workflow knowledge

This assistant is designed to deliver operational guidance while keeping the experience seamless inside the hospital management UI.

### AI Assistant Workflow

```text
User asks a question
  ↓
Browser chat widget
  ↓
ASP.NET ChatController
  ↓
FastAPI /ask endpoint
  ↓
Query enrichment + vector retrieval
  ↓
Gemini LLM generation
  ↓
JSON response returned
  ↓
Assistant reply appears in UI
```

---

## System Architecture

The platform uses a hybrid architecture with a monolithic ASP.NET MVC core and a separate AI service.

```text
Browser / Razor UI
  ↓
ASP.NET MVC Controllers
  ↓
EF Core / SQLite

Chat Widget
  ↓
ChatController
  ↓
FastAPI AI Service
  ↓
RAG Pipeline
  ↓
Gemini LLM
```

### Architecture highlights

- **ASP.NET MVC** for server-rendered UI and role-driven workflows
- **Entity Framework Core** for data access and relationship management
- **SQLite** for lightweight, portable storage in development
- **FastAPI** for the conversational AI microservice
- **AI retrieval workflow** for relevant assistant responses

---

## Technology Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core MVC, C# (.NET 9) |
| Database | SQLite, Entity Framework Core |
| AI Service | FastAPI, Python, SentenceTransformers, FAISS, Google Gemini |
| Frontend | Razor Views, Bootstrap 5, jQuery, custom JS chat widget |
| Styling | Bootstrap 5, responsive layout, modern dashboard UI |
| Tooling | dotnet CLI, EF Core Migrations, conda/Python, uvicorn |

---

## Database Design

The system uses a clean domain model with strong relationships:

- **Users**: staff identities with `Admin`, `Doctor`, and `Reception` roles.
- **DoctorProfiles**: doctor metadata, specialty, room, and linked user account.
- **Patients**: patient demographics, contact details, and active status.
- **Appointments**: scheduled patient-doctor slots with status tracking, conflict validation, and duration.
- **Visits**: clinical records generated manually or via completed appointment workflow.

### Relationships

- One `User` → one `DoctorProfile`
- One `Patient` → many `Appointments`
- One `DoctorProfile` → many `Appointments`
- One `Patient` → many `Visits`
- One `DoctorProfile` → many `Visits`

---

## UI / User Experience

- **Role-aware navigation** keeps users focused on their tasks.
- **Floating AI assistant** is available across all authenticated pages.
- **Dashboard cards** provide quick insight into daily operations.
- **Responsive layout** supports desktop workflows with clear forms and tables.
- **Workflow-driven pages** guide users from patient intake to appointment completion.

---

## Project Structure

```text
HospitalInformationSystem/
├─ Controllers/
│  ├─ AdminController.cs
│  ├─ AppointmentsController.cs
│  ├─ AuthController.cs
│  ├─ ChatController.cs
│  ├─ DoctorController.cs
│  ├─ DoctorsController.cs
│  ├─ HomeController.cs
│  ├─ PatientsController.cs
│  ├─ ReceptionController.cs
│  ├─ UsersController.cs
│  └─ VisitsController.cs
├─ Data/
│  ├─ ApplicationDbContext.cs
│  └─ SeedData.cs
├─ Helpers/
│  └─ PasswordHelper.cs
├─ Models/
│  ├─ Appointment.cs
│  ├─ DoctorProfile.cs
│  ├─ Patient.cs
│  ├─ User.cs
│  ├─ UserProfile.cs
│  ├─ Visit.cs
│  └─ status enums
├─ ViewModels/
│  ├─ LoginViewModel.cs
│  ├─ CRUD_AppointmentViewMode/
│  ├─ CRUD_DoctorProfileViewMode/
 │  ├─ CRUD_PatientViewMode/
│  ├─ CRUD_UserViewModel/
│  ├─ CRUD_VisitViewMode/
│  └─ Dashboard/
├─ Views/
│  ├─ Shared/
│  ├─ Admin/
│  ├─ Doctor/
│  ├─ Reception/
│  ├─ Patients/
│  ├─ Users/
│  ├─ Appointments/
 │  └─ Visits/
├─ wwwroot/
│  ├─ css/
│  └─ js/floating-chat/
├─ RAG_Chat/
│  └─ main.py
├─ Migrations/
├─ appsettings.json
├─ Program.cs
└─ start-project.ps1
```

### Folder responsibilities

- `Controllers/`: core workflow endpoints and business orchestration
- `Data/`: EF Core context and seed initialization
- `Models/`: domain entities and relationships
- `ViewModels/`: form validation and typed input models
- `Views/`: Razor UI pages and shared layout
- `wwwroot/`: frontend assets, chat widget scripts, and styling
- `RAG_Chat/`: AI assistant service and retrieval pipeline

---

## Installation & Setup

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Python 3.11+](https://www.python.org/downloads/)
- [conda](https://docs.conda.io/projects/conda/en/latest/user-guide/install/index.html)
- `uvicorn`, `fastapi`, `sentence-transformers`, `faiss-cpu`, `google-generativeai`

### Setup Steps

1. **Clone repository**

```bash
git clone <repository-url>
cd HospitalInformationSystem
```

2. **Configure Python environment**

```powershell
conda create -n genai python=3.11 -y
conda activate genai
pip install fastapi uvicorn sentence-transformers faiss-cpu google-generativeai
```

3. **Set Gemini API key**

```powershell
$env:GEMINI_API_KEY="your_gemini_api_key"
```

4. **Run migrations and seed DB**

```powershell
dotnet ef database update
```

5. **Start FastAPI assistant service**

```powershell
cd RAG_Chat
uvicorn main:app --host 0.0.0.0 --port 8000
```

6. **Start ASP.NET app**

```powershell
dotnet run
```

7. **Alternative startup**

```powershell
.\start-project.ps1 -CondaEnv "genai"
```

---

## Default Accounts

| Role | Username | Email | Password |
|---|---|---|---|
| Admin | `admin` | `admin@his.com` | `123456` |
| Doctor | `doctor1` | `doctor1@his.com` | `123456` |
| Reception | `reception1` | `reception1@his.com` | `123456` |

> These seeded accounts are included for demonstration and development. Update credentials before production use.

---

## Workflow Showcase

### 1. Patient Appointment Flow

1. Reception logs in and registers a new patient.
2. Reception schedules an appointment with a doctor.
3. Doctor views the appointment on their dashboard.
4. Doctor confirms and completes the appointment.
5. The system creates a completed visit record automatically.

### 2. Doctor Care Workflow

1. Doctor logs in and reviews today’s schedule.
2. Doctor opens patient appointment details.
3. Doctor updates visit notes and status.
4. Completed visits are tracked in the doctor timeline.

### 3. AI-Assisted Guidance

1. User opens the floating AI assistant.
2. User submits a workflow or role question.
3. Assistant forwards the query to FastAPI.
4. AI returns a contextually relevant answer in the UI.

---

## Future Improvements

- **Cloud-ready deployment** with Docker and container orchestration
- **Advanced analytics** with charting and trend reporting
- **Notification engine** for appointment reminders and alerts
- **Expanded AI knowledge base** with medical protocols and help content
- **Multi-tenant support** for hospital groups or clinics
- **Integration APIs** for external scheduling, billing, or EMR systems

---

## Screenshots

### Login
![Login](docs/screenshots/login.png)

### Dashboard
![Dashboard](docs/screenshots/dashboard.png)

### Appointments
![Appointments](docs/screenshots/appointments.png)

### Patients
![Patients](docs/screenshots/patients.png)

### AI Assistant
![AI Assistant](docs/screenshots/ai-assistant.png)

### Reception Panel
![Reception](docs/screenshots/reception.png)

---

## Contributing

Contributions are welcome. Please use a clear issue description, link to the relevant module, and include screenshots or repro steps when appropriate.

1. Fork the repository
2. Create a feature branch
3. Submit a pull request with details and testing notes

---

## License

Distributed under the **MIT License**. See `LICENSE` for details.

---

## Final Note

This repository demonstrates a professional, full-stack healthcare solution combining solid ASP.NET MVC design with modern AI service integration. It showcases operational workflows, role-based controls, and an intelligent assistant aligned to real hospital use cases.
