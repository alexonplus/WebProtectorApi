# WebProtector - Fullstack Security Scanner

## 📝 Description
WebProtector is a fullstack application for security analysis. Users can register, log in, and perform security scans on URLs. The results are stored in a Microsoft SQL Server database and managed through a dashboard.

## 🚀 How to Run
1. **Backend:** - Open the solution in Visual Studio.
   - Run the project (F5) to start the API at `https://localhost:7290`.
2. **Frontend:** - Open the frontend folder in VS Code.
   - Run `index.html` using **Live Server** (Port 5500).

## 🔗 API Endpoints
- `POST /api/Auth/register` - User registration.
- `POST /api/Auth/login` - Authentication and JWT issuance.
- `POST /api/Scanner/scan` - Execute security scan for a URL.
- `GET /api/Scanner/reports` - Fetch scan history.
- `DELETE /api/Scanner/report/{id}` - Delete a specific report.

## ⚙️ Frontend & API Integration
The Frontend communicates with the API using the **JavaScript Fetch API**. 
- It sends JSON data to the backend endpoints.
- It manages authentication by storing the **JWT Token** in `localStorage` and including it in the `Authorization` header for API requests.

## 🧠 Reflection
- **What went well:** Core CRUD functionality (Create, Read, Delete) was successfully implemented. The connection between the Frontend and the SQL Server database is stable and functional.
- **Challenges:** Configuring **Swagger** documentation and managing **JWT authentication settings** across different environment versions was difficult. Significant troubleshooting was required to handle **CORS policies** and ensure seamless communication between Port 5500 (Frontend) and Port 7290 (Backend).
