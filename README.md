# 🏭 MES — Manufacturing Execution System

> A web-based system for managing manufacturing operations, from production requirements and planning to resource coordination and shop-floor progress monitoring, with an AI-powered assistant for querying MES data and uploaded documents.

![MES API](https://img.shields.io/badge/MES%20API-.NET%2010-512BD4)
![Chatbot API](https://img.shields.io/badge/Chatbot%20API-.NET%208-512BD4)
![Frontend](https://img.shields.io/badge/Frontend-React%2018-61DAFB)
![Database](https://img.shields.io/badge/Database-SQL%20Server-CC2927)

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Repository Structure](#-repository-structure)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [Database](#-database)
- [Security](#-security)
- [Verification Status](#-verification-status)
- [Roadmap](#-roadmap)

---

## 🎯 Overview

MES supports production planning and shop-floor operations through a React frontend and a set of ASP.NET Core services.

The repository currently contains:

- The main MES API and its application, domain, infrastructure, scheduling, and ML projects.
- A separate AI chatbot API that uses retrieval-augmented generation (RAG), SQL Server, Redis, Qdrant, and an OpenAI-compatible provider.
- A React web client.
- A SQL Server initialization script containing the MES schema and sample data.

---

## ✨ Key Features

### Manufacturing Execution

- Manage production requirements and manufacturing orders.
- Plan and schedule work orders.
- Manage materials, bills of materials, formulas, and production parameters.
- Manage equipment, equipment capabilities, work centers, and facilities.
- Manage personnel and personnel capabilities.
- Record work-order progress, production errors, and equipment incidents.
- View production dashboards and OEE-related reports.
- Receive production progress updates through SignalR.
- Use scheduling algorithms and machine-learning components for planning and prediction.

### AI Chatbot

- Ask questions about MES data through an authenticated chat endpoint.
- Use retrieval-augmented generation with documents indexed in Qdrant.
- Ingest supported PDF, text, Markdown, and Excel files.
- Cache chat history and query results using Redis.
- Generate selected data exports and chart responses.

---

## 🏗️ Architecture

```text
┌──────────────────────────┐
│ React + Vite Frontend    │
│ repository: frontend/    │
└─────────────┬────────────┘
              │
              ├── REST API / SignalR
              │
       ┌──────┴───────────────┐
       ▼                      ▼
┌───────────────────┐  ┌────────────────────┐
│ MES API           │  │ MES Chatbot API    │
│ ASP.NET Core 10   │  │ ASP.NET Core 8     │
│ backend/MES.Api   │  │ backend/MESChatbot │
└─────────┬─────────┘  └───────┬────────────┘
          │                    │
          ▼                    ├── SQL Server (read-only account recommended)
   ┌──────────────┐             ├── Redis
   │ SQL Server   │             ├── Qdrant
   │ database/    │             └── OpenAI-compatible API
   └──────────────┘
```

### Main MES Backend Projects

| Project | Responsibility |
|---|---|
| `MES.Api` | Main ASP.NET Core API, controllers, middleware, and configuration |
| `MES.Application` | Use cases, CQRS handlers, DTOs, policies, and application services |
| `MES.Domain` | Domain entities, value objects, enums, and repository contracts |
| `MES.Infrastructure` | EF Core DbContext, entity configuration, repositories, and ML infrastructure |
| `MES.Algorithm` | Scheduling and resource-assignment algorithms |
| `MES.MLTrainer` | Data processing and machine-learning training/prediction utilities |

### Chatbot Service

The chatbot is a separate ASP.NET Core application at `backend/MESChatbot/AIChatbot.Api`. It exposes chat and document-ingestion endpoints and depends on SQL Server, Redis, Qdrant, and an OpenAI-compatible API provider.

---

## 🛠️ Technology Stack

### MES Backend

- .NET 10 and ASP.NET Core
- Entity Framework Core and SQL Server
- MediatR and CQRS
- AutoMapper
- JWT authentication with refresh-token cookies
- SignalR
- ML.NET
- Docker and Docker Compose

### Chatbot Backend

- .NET 8 and ASP.NET Core
- OpenAI .NET client
- SQL Server client
- Redis distributed cache
- Qdrant vector database
- ClosedXML and PdfPig for document processing

### Frontend

- React 18
- Vite
- Axios
- Microsoft SignalR client
- Chart.js, ApexCharts, and Recharts
- CSS and SCSS modules

### Database and Supporting Services

- Microsoft SQL Server
- Redis
- Qdrant
- Entity Framework Core migrations
- SQL initialization and sample data script in `database/MESbackup.sql`

---

## 📂 Repository Structure

```text
MES/
├── backend/
│   ├── MES.Api/
│   ├── MES.Application/
│   ├── MES.Domain/
│   ├── MES.Infrastructure/
│   ├── MES.Algorithm/
│   ├── MES.MLTrainer/
│   ├── MES.slnx
│   ├── docker-compose.yml
│   └── MESChatbot/
│       ├── AIChatbot.sln
│       ├── docker-compose.yml
│       └── AIChatbot.Api/
├── database/
│   └── MESbackup.sql
└── frontend/
    ├── src/
    ├── package.json
    └── vite.config.js
```

---

## ✅ Prerequisites

- .NET 10 SDK for the main MES backend.
- .NET 8 SDK for the chatbot API.
- Node.js and npm for the frontend.
- SQL Server accessible to the MES API and chatbot.
- Docker Desktop for Redis, Qdrant, or containerized API services.
- An OpenAI-compatible API key for chatbot chat and embedding features.

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/huy1976/MES.git
cd MES
```

### 2. Initialize the Development Database

Create a SQL Server database named `MES`, then run `database/MESbackup.sql` using SQL Server Management Studio or `sqlcmd`.

The script creates the database if it does not already exist, then creates its schema and inserts sample data. The SQL Server login must have permission to create the database and database objects.

Review the script before running it. Do not run the sample-data script against a production database.

### 3. Configure and Run the Main MES API

Set the SQL Server connection string and JWT signing key using .NET User Secrets or environment variables. Do not put real credentials in a committed `appsettings.json`.

From the repository root:

```powershell
cd backend
dotnet restore .\MES.slnx
dotnet run --project .\MES.Api\MES.Api.csproj
```

Check the application console output or `backend/MES.Api/Properties/launchSettings.json` for the local HTTPS address and port.

### 4. Run the Main MES API with Docker Compose

The main backend Compose file builds and runs the MES API. It requires a SQL Server connection string and JWT signing key.

From `backend/`:

```powershell
$env:MES_DB_CONNECTION_STRING = "Server=YOUR_SERVER;Database=MES;..."
$env:MES_JWT_KEY = "REPLACE_WITH_A_LONG_RANDOM_SECRET"
docker compose up --build
```

The Compose configuration publishes the MES API on port `8080`.

### 5. Run Chatbot Dependencies

The chatbot Compose file starts Redis and Qdrant. From the chatbot folder:

```powershell
cd backend\MESChatbot
docker compose up -d
```

The current Compose file exposes Redis on port `6379` and Qdrant on ports `6333` and `6334`.

### 6. Configure and Run the Chatbot API

Set chatbot SQL, JWT, and OpenAI API settings locally using User Secrets or environment variables. Do not commit real keys or connection strings.

Then run:

```powershell
cd backend\MESChatbot
dotnet restore .\AIChatbot.sln
dotnet run --project .\AIChatbot.Api\AIChatbot.Api.csproj
```

The local HTTPS profile is currently configured for port `7260`; check `AIChatbot.Api/Properties/launchSettings.json` for the active profile.

### 7. Run the Frontend

Open a separate terminal:

```powershell
cd frontend
npm install
npm run dev
```

Open the local URL printed by Vite, usually `http://localhost:5173`.

---

## ⚙️ Configuration

Configure environment-specific values locally or through your deployment secret store. Do not commit real credentials.

### Main MES API

| Setting | Purpose |
|---|---|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string for the MES API |
| `Jwt__Key` | Secret used to sign MES access tokens |
| `Jwt__Issuer` | MES JWT issuer |
| `Jwt__Audience` | MES JWT audience |
| `MES_DB_CONNECTION_STRING` | Required database connection string for the main API Compose service |
| `MES_JWT_KEY` | Required JWT signing key for the main API Compose service |

### Chatbot API

| Setting | Purpose |
|---|---|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string for chatbot data queries |
| `OpenAI__ApiKey` | API key for the configured OpenAI-compatible provider |
| `OpenAI__ChatModel` | Chat model name |
| `OpenAI__EmbedModel` | Embedding model name |
| `Jwt__Key` | Key used to validate MES-issued JWTs |
| `Jwt__Issuer` | Expected MES token issuer |
| `Jwt__Audience` | Expected MES token audience |
| `Redis__Connection` | Redis host and port |
| `Qdrant__Endpoint` | Qdrant HTTP endpoint |
| `Qdrant__Collection` | Qdrant collection base name |

The chatbot validates JWT issuer, audience, lifetime, and signature. Its JWT configuration must match the main MES API.

The frontend and backend currently contain local-development URLs. Configure the correct API URLs and CORS origins before deploying to another environment.

---

## 🗄️ Database

`database/MESbackup.sql` is a SQL Server script containing the MES database schema and sample data.

The sample data includes production orders, work orders, materials, equipment, personnel, permissions, and progress records. Review and sanitize it before sharing the repository publicly or using it outside a development environment.

The chatbot queries MES data. Use a dedicated SQL login with only the permissions it requires; do not use a database owner or administrator account for routine chatbot access.

---

## 🔐 Security

- Never commit real connection strings, passwords, OpenAI API keys, JWT signing keys, access tokens, refresh tokens, or private certificates.
- Use .NET User Secrets for local development and environment variables or a secret manager for deployment.
- Replace all placeholder values before starting the services.
- Rotate any credential that has been committed, logged, or otherwise exposed.
- The chatbot chat endpoint requires JWT authentication. Review the document-ingestion endpoint's access policy before exposing the service beyond a trusted development environment.
- Review SQL scripts and sample data before running them; do not execute them against production without a verified backup and an explicit migration plan.

---

## 🧪 Verification Status

- The frontend production build completed successfully in the development environment.
- Frontend lint currently reports existing errors and warnings that still need to be addressed.
- Backend and chatbot startup depend on configured SQL Server, JWT, Redis, Qdrant, and model-provider settings.
- Validate the full frontend-to-MES API, frontend-to-chatbot, and service-to-database flows in the target environment before deployment.

---

## 🗺️ Roadmap

- [ ] Centralize frontend API and service URLs by environment.
- [ ] Complete and harden authentication, session restoration, and logout flows.
- [ ] Resolve frontend lint issues and split large production bundles.
- [ ] Review and update dependencies with security advisories.
- [ ] Add repeatable backend, chatbot, and frontend test instructions.
- [ ] Document production deployment, database migration, backup, and recovery procedures.
- [ ] Sanitize development data and provide a safe sample dataset.
- [ ] Document chatbot model-provider, Redis, Qdrant, and document-ingestion setup.

---

<div align="center">

**MES — Manufacturing Execution System**

</div>
