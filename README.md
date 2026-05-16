# Ecommerce API

A modern, robust, and scalable E-commerce backend API built with **ASP.NET Core 10** following the **Clean Architecture** principles.

## 🏗️ Architecture

The solution is divided into three main layers to separate concerns, improve maintainability, and ensure scalability:

1. **Ecommerce.Core**: 
   - The heart of the application.
   - Contains Domain Entities (`Product`, `Category`, `Order`, `Payment`, `AppUser`, `Review`).
   - Defines abstract interfaces (Contracts/Repositories) and DTOs.
   - Completely independent of data access or UI concerns.

2. **Ecommerce.Infrastructure**:
   - Implements the interfaces defined in the Core layer.
   - Contains Data Access Logic (`AppDbContext`, Repositories).
   - Manages integrations with external services (Email, Payment, Image Storage).
   - Handles Database Migrations and Seeding.

3. **Ecommerce.Api**:
   - The presentation layer.
   - Contains RESTful Controllers (`AuthController`, `ProductsController`, `OrdersController`, `PaymentsController`, `WebhooksController`, etc.).
   - Registers Dependency Injections and Configures the HTTP Request Pipeline.

## 🚀 Key Features

- **Authentication & Authorization**: Secure identity management using ASP.NET Core Identity, JWT (JSON Web Tokens), and a robust OTP (One Time Password) flow for two-step verification.
- **Role-Based Access**: Distinguishes between Admin and Customer capabilities.
- **Product & Category Management**: Full CRUD operations for managing catalogs.
- **Order Processing**: Seamless order placement, tracking, and item management.
- **Payment Gateway Integration**: Built-in support for **Paymob** to handle transactions and webhooks.
- **Media Management**: Integrates with **Cloudinary** for scalable image hosting.
- **Email Notifications**: Asynchronous email delivery via **MailKit** and SMTP.
- **Security & Reliability**: Built-in API Rate Limiting (preventing abuse), Global Exception Handling, and comprehensive Model Validation.

## 🛠️ Technology Stack

- **Framework**: .NET 10.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core 10
- **Security**: ASP.NET Core Identity & JWT
- **API Documentation**: Swagger (OpenAPI) & Scalar
- **Object Mapping**: AutoMapper
- **Third-Party Services**:
  - **Paymob** (Payments)
  - **Cloudinary** (Images)
  - **MailKit** (Emails)
- **Caching**: In-Memory Cache (for OTPs and fast data retrieval)

## ⚙️ Setup & Configuration

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/M-Khailfa/Ecommerce
   cd Ecommerce
   ```

2. **Configure AppSettings**:
   Ensure you have configured `appsettings.json` or `appsettings.Development.json` in the `Ecommerce.Api` project with the following required sections:
   - `ConnectionStrings:EcommerceDb`: Your SQL Server connection string.
   - `JWT`: Secret Key, Issuer, Audience.
   - `Paymob`: API Key, Integration IDs.
   - `CloudinarySettings`: Cloud Name, API Key, API Secret.
   - `SmtpSettings`: Server, Port, User, Password.

3. **Database Migration**:
   Update the database to apply the latest EF Core migrations. From the API directory, run:
   ```bash
   dotnet ef database update --project ../Ecommerce.Infrastructure
   ```

4. **Run the Application**:
   ```bash
   dotnet run --project Ecommerce.Api
   ```

5. **API Documentation**:
   Navigate to the base URL to explore the API endpoints:
   - Swagger UI: `https://localhost:<port>/swagger`
   - Scalar UI: `https://localhost:<port>/scalar`

## 🛡️ License

This project is licensed under the MIT License.
