This project is a full-stack **Phone Book** application designed with a modern **React (Vite/TypeScript)** frontend and a robust **.NET 8 Web API** backend. It uses an **Oracle Database** for persistence and supports full CRUD operations, pagination, and real-time search.

---

## 🚀 Getting Started

The easiest way to run the entire stack (Frontend, Backend, and Database) is using **Docker Compose**.

### Prerequisites

* [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.
* (Optional) [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and [Bun](https://bun.sh/) if you wish to run services locally without Docker.

### Running with Docker 🐳

1. Clone this repository to your local machine.
2. Open a terminal in the project root directory.
3. Run the following command:
```bash
docker-compose up --build

```


4. **Wait for the database to initialize.** The Oracle XE container may take 1-2 minutes to become "healthy." The backend service is configured to wait for this health check before starting.
5. **Access the application:**
* **Frontend:** [http://localhost:5173](https://www.google.com/search?q=http://localhost:5173)
* **Backend API (Swagger):** [http://localhost:5000/swagger](https://www.google.com/search?q=http://localhost:5000/swagger)
* **Health Check:** [http://localhost:5000/health](https://www.google.com/search?q=http://localhost:5000/health)



---

## 🛠️ Project Structure

The codebase is organized into three main areas:

### 1. Backend (`/backend`)

An **ASP.NET Core 8 API** following clean architecture principles:

* **Controllers:** RESTful endpoints for Contact management.
* **Services:** Business logic, including a `DeletionLogService` that writes records to a local text file whenever a contact is removed.
* **Repositories:** Data access layer using **Entity Framework Core** with the Oracle provider.
* **DTOs:** Data Transfer Objects for clean API contracts.
* **Tests:** Unit tests using **xUnit**, **Moq**, and **FluentAssertions**.

### 2. Frontend (`/frontend`)

A **React 18** application built with:

* **TypeScript:** For type safety.
* **Tailwind CSS:** For styling, utilizing a custom UI kit (buttons, cards, dialogs).
* **Lucide React:** For iconography.
* **Hooks:** Custom hooks like `useContacts` for state management and `useConfirmDialog` for reusable UI logic.

### 3. Database (`/db`)

* **Migrations:** SQL scripts to initialize the `CONTACT` and `PHONE` tables in Oracle.
* **Relationships:** A One-to-Many relationship between Contacts and Phone Numbers with cascade delete enabled.

---

## 📖 Features & Usage

### 🔍 Search & Pagination

* Use the search bar to filter contacts by **Name** or **Phone Number**. The backend uses indexed `UPPER()` searches for performance.
* Results are paginated (default 8 per page) to ensure fast loading times.

### 📝 Contact Management

* **Add:** Click the "Add" button to create a new contact. You must provide a name, age (1-149), and at least one phone number.
* **Edit:** Modify existing contact details or manage multiple phone numbers for a single entry.
* **Delete:** Remove a contact. A confirmation dialog prevents accidental deletions.

### 📋 Logging

Every time a contact is deleted, the backend logs the action (including the timestamp and deleted data) to:
`backend/logs/deletion_log.txt` (or `/app/logs/` inside the Docker container).

---

## 🧪 Running Tests

To run the backend unit tests:

```bash
cd backend
dotnet test

```

---

## ⚙️ Environment Variables

The project includes a `.env.example` file. For local development outside of Docker, create a `.env` file in the root:

| Variable | Description | Default |
| --- | --- | --- |
| `ORACLE_CONNECTION_STRING` | Connection string for Oracle DB | - |
| `VITE_API_URL` | The URL of the Backend API | `http://localhost:5000` |
| `CORS_ALLOWED_ORIGINS` | Allowed origins for API access | `http://localhost:5173` |
| `DELETION_LOG_PATH` | Path where deleted contacts are logged | `./logs/deletion_log.txt` |

Would you like me to help you set up specific CI/CD pipelines for this project or explain the Oracle EF Core configuration in more detail?
