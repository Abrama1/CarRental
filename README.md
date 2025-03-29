# 🚗 Car Rental API

A full-featured ASP.NET Core Web API for managing a car rental service. This system supports user registration, authentication, admin/customer roles, car listings (with image upload), rental creation, email notifications, and JWT-based security.

---

## 📦 Features

- ✅ JWT Authentication with Role-based Authorization
- 👥 Separate roles for `Admin` and `Customer`
- 📩 Email verification & rental status notifications
- 🚘 Car listing with image upload & availability control
- 📆 Rental creation, updating, canceling, history tracking
- 🖼️ Multiple image support stored in `wwwroot/uploads`
- 📃 Swagger UI for testing endpoints

---

## 🛠️ Tech Stack

- **Backend:** ASP.NET Core 8 Web API
- **ORM:** Entity Framework Core
- **Database:** Microsoft SQL Server
- **Auth:** JWT Bearer Tokens
- **Mail:** SMTP (via Gmail)
- **API Docs:** Swagger/OpenAPI
- **Storage:** Local image hosting (`wwwroot/uploads`)

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/yourusername/CarRentalAPI.git
cd CarRentalAPI
```

### 2. Configure `appsettings.json`

Update JWT, DB, and email settings:

```json
"JwtSettings": {
  "Key": "your-super-secret-key",
  "Issuer": "CarRentalAPI",
  "Audience": "CarRentalUsers",
  "ExpireMinutes": 60
},
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=CarRentalDb;Trusted_Connection=True;TrustServerCertificate=True;"
},
"EmailSettings": {
  "Sender": "carrentalalemailservice@gmail.com",
  "Password": "your-app-password"
}
```

### 3. Run Migrations (if needed)

```bash
dotnet ef database update
```

### 4. Launch the app

```bash
dotnet run
```

Visit: `https://localhost:7229/swagger`

---

## 🔐 Authentication

- Customers register with email & password
- Admins can log in directly via `/api/admin/login`
- JWT tokens are returned and should be added to Swagger via the **Authorize** button (🔒)

---

## 🧪 API Testing (Swagger)

- `POST /api/customer/register` – Register user
- `POST /api/customer/login` – Authenticate and get token
- `POST /api/car/add` – Add a new car (with images)
- `POST /api/rental/create` – Book a rental
- `PATCH /api/admin/approve/{id}` – Approve rental (admin)
- `DELETE /api/car/delete/{id}` – Remove car and images

---

## 📷 Car Image Uploads

- Upload multiple files during **Car creation** or **update**
- Images saved under `/wwwroot/uploads`
- Full URLs are included in responses (e.g., `https://localhost:7229/uploads/xxx.png`)
- On delete, all car images are also removed

---

## 📧 Email Notifications

- 📨 Verification email sent during customer registration
- 📨 Rental approved/declined notifications sent to customer
- 📨 Admin notified when customer updates a rental

---

## 🧑‍💻 Roles

| Role     | Description                        | Permissions                       |
|----------|------------------------------------|-----------------------------------|
| Customer | Default registered user            | Can view cars, create rentals     |
| Admin    | Manually added in DB               | Full access, approve/cancel/etc.  |

---

## 🗃️ Database Overview

- **Customers** – Registration, Login, JWT, Rentals
- **Cars** – Multi-image support, availability
- **Rentals** – Booking logic, status, price calculation

---

## 🛡️ Security Notes

- Passwords are hashed using **BCrypt**
- JWT claims include Role, Email, and ID
- Role-checking enforced in all sensitive controllers

---
