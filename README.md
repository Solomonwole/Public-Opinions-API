# Opinions API

A full-stack backend API built with **ASP.NET Core (.NET 9)** that allows users to register, authenticate, and create public opinions with proper ownership-based authorization.

This project demonstrates real-world backend practices including JWT authentication, email verification, secure password handling, pagination, and RESTful API design.

---

## 🚀 Features

### Authentication & Security
- User registration
- Email verification
- Login with JWT authentication
- Forgot password & password reset
- Secure password hashing (BCrypt)
- Protected routes using JWT
- Ownership-based authorization

### Opinions
- Create a public opinion (authenticated users)
- Retrieve all public opinions (paginated)
- Edit opinions created by the user
- Delete opinions created by the user

---

## 🛠 Tech Stack

**Backend**
- ASP.NET Core Web API (.NET 9)
- Entity Framework Core
- PostgreSQL
- JWT Authentication

**Tooling**
- Visual Studio 2022 (Windows)
- pgAdmin
- Swagger / OpenAPI
- Postman (API testing)

---
## 📐 API Design

- RESTful endpoints
- Clear separation of concerns
- DTO-based request/response models
- Pagination support with metadata
- Proper HTTP status codes

---

## 🔐 Authentication Flow

1. User registers with email & password
2. Email verification is required before login
3. Login returns a JWT token
4. JWT must be sent in the `Authorization` header: Authorization: Bearer <token>
5. Protected endpoints enforce authentication and ownership rules

---

## 📄 Pagination

Public opinions endpoint supports pagination:

GET /api/opinions?page=1&pageSize=10


Response includes:
- Current page
- Page size
- Total count
- Total pages
- Paginated data

---
## ▶️ Running the Project Locally

### Prerequisites
- .NET 9 SDK
- PostgreSQL
- Visual Studio 2022

### Db and JWT Setup
Configure Secrets (Required)

Secrets are not committed to the repository.

Use User Secrets for local development:

```
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=opinionsdb;Username=postgres;Password=YOUR_PASSWORD"
dotnet user-secrets set "Jwt:Key" "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARS"
```

### 🧪 Testing

Swagger UI for manual testing

Postman for end-to-end API testing

JWT-protected endpoints require authorization


### 📌 Project Purpose

This project was built to demonstrate:

Secure authentication & authorization

Ownership-based access control

Real-world backend API design

Clean architecture and best practices

### 📬 Future Improvements

Email sending via SMTP or SendGrid

Refresh tokens

Search & sorting for opinions

Role-based authorization

Frontend integration (Next.js)


### 👤 Author

Tosin Adewole
Full-Stack Developer
GitHub: https://github.com/solomonwole