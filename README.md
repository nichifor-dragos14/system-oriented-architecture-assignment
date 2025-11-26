# 🎓 **SOA — Student & Professor Management System**

This project demonstrates a **microservices-based academic platform**, integrating multiple backend services, event-driven communication, and two **Angular 19** frontends — one for students and one for professors.  
All components are containerized using **Docker Compose** 🐳 and orchestrated via **NGINX** 🌐 as the main entry point.

---

## 🧱 **System Architecture Overview**

| Layer | Component | Technology | Description |
|-------|------------|-------------|--------------|
| 🌐 **Edge Layer** | **NGINX Reverse Proxy** | `nginx:alpine` | Serves static frontends and proxies all `/api/*` requests to the backend Gateway. Enables SSE for real-time GPA streaming. |
| 🧠 **Gateway** | **SOA.Gateway** | `.NET 8 Web API` | Secured REST API façade handling JWT authentication, request routing, and aggregation. |
| 🧩 **Microservices** | **SOA.GradeService** | `.NET 8 Microservice` | CRUD operations for grades; publishes events to RabbitMQ. |
|  | **SOA.StudentService** | `.NET 8 Microservice` | Manages student information and provides lookup services. |
|  | **SOA.NotificationService** | `.NET 8 Worker + SSE**` | Listens to RabbitMQ and broadcasts real-time GPA updates via Server-Sent Events. |
| ⚙️ **FaaS Layer** | **SOA.Functions (GPA Calculator)** | `.NET 8 Minimal Function**` | Function-as-a-Service component calculating GPA updates triggered by events. |
| 💾 **Database** | **PostgreSQL** | `postgres:16` | Persistent store for students and grades. |
| 🗂️ **Admin Tools** | **pgAdmin** | `dpage/pgadmin4` | Web interface for managing PostgreSQL. |
| 🐇 **Message Broker** | **RabbitMQ** | `rabbitmq:3.13-management` | Enables asynchronous event communication between services. |
| 🔁 **Event Stream** | **Redpanda (Kafka compatible)** | `redpandadata/redpanda` | Provides event-streaming capability for GPA and analytics. |
| 🧑‍💻 **Frontend - Professors** | **Angular SPA (professor-ui)** | `/professors/` | Professors can view students, add/edit/delete grades. |
| 🧑‍🎓 **Frontend - Students** | **Angular SPA (student-ui)** | `/students/` | Students can log in, view grades, and receive live GPA updates via SSE. |

---

## 🏗️ **Build Instructions**

### 🧩 Step 1 — Build the Angular Frontends
```bash
# From project root
cd soa-frontend

# Build Student UI
ng build student-ui --configuration production 

# Build Professor UI
ng build professor-ui --configuration production 

# Return to project root
cd ../..
```

### 🐋 Step 2 — Start All Containers
```bash
docker compose up --build
```
#### For load balancing

```bash
docker compose up -d --scale gateway=3
```

## 🧭 Step 3 — Access the System

| Service                   | URL                                                          | Description                             |
| ------------------------- | ------------------------------------------------------------ | --------------------------------------- |
| 🧑‍🎓 **Student UI**      | [http://localhost/students/](http://localhost/students/)     | Student dashboard and GPA viewer        |
| 🧑‍🏫 **Professor UI**    | [http://localhost/professors/](http://localhost/professors/) | Grade management interface              |
| 🧩 **Gateway API**        | [http://localhost/swagger/index.html](http://localhost/swagger/index.html)               | REST API + SSE endpoints                |
| 🐘 **pgAdmin**            | [http://localhost:5050](http://localhost:5050)               | (user: `admin@admin.com`, pass: `root`) |
