# 🎉 Event Management System

A full-stack Event Management web application built using **ASP.NET Core MVC** and **Entity Framework Core**, designed to manage events, registrations, and integrate with an external ML-based recommendation service.

---

## 🚀 Features

🚀 Key Features
Clean Architecture: Implemented using the Repository Pattern to ensure a decoupled and testable codebase.

Role-Based Access Control (RBAC): Secure authentication using JWT and identity management for Admins, Organizers, and Attendees.

Financial Integration: Real-world payment simulation using Stripe API with automated PDF receipt generation.

Background Processing: Utilizes Hangfire for asynchronous tasks such as sending bulk emails and processing registrations without lagging the UI.

AI-Powered Insights: (External) Integrates with a Python-based Recommender System hosted on Azure to suggest events to users.

Cloud Ready: Fully configured for Azure App Service and Azure SQL Database.

---

## 🏗️ Tech Stack

**Backend**
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server / Azure SQL

**Frontend**
- Razor Views
- Bootstrap

**External Integration**
- FastAPI ML Recommendation Service
- REST API Communication using HttpClient

🏗 System Architecture
The project follows a layered approach to maintain the S.O.L.I.D. principles:

Core/Domain: Models and Entities.

Infrastructure: Data context, Migrations, and Repository implementations.

Services: Business logic, Email automation, and Stripe integration.

Web/Presentation: Controllers, Views

