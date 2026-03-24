🛡️ WebProtector - Fullstack Security Scanner
📝 Description
WebProtector is a professional fullstack application designed for automated security analysis of web resources. It provides a complete flow for users to register, authenticate, and perform deep security scans on any URL.

🧠 Smart Scoring Logic (The "Brain")
Unlike basic scanners, WebProtector evaluates risks and calculates a Security Score and Grade:

Base Score: Every scan starts with a perfect 100 points.

Automated Grading: The system assigns a letter grade (A, B, C, D, or F) based on the final calculated score.

Risk-Based Penalties:

-40 points: If the site uses insecure HTTP instead of HTTPS.

-20 points: For missing critical security headers like X-Frame-Options or Content-Security-Policy.

-10 points: For missing X-Content-Type-Options.

🚀 Key Features
JWT Authentication: Secure login and registration using JSON Web Tokens stored in localStorage.

Real-time Analysis: Performs local checks on target URLs and returns detailed reports.

Scan History: Users can view their previous scans and manage their report history.

Secure API: All scanner endpoints are protected by authentication to ensure data privacy.

🛠 Tech Stack
Backend: ASP.NET Core Web API, Entity Framework Core.

Database: Microsoft SQL Server.

Frontend: Modern JavaScript (ES6+), Fetch API, CSS3.

⚙️ Setup & Run
Backend: Open the solution in Visual Studio and run (F5). The API typically runs at https://localhost:7290.

Frontend: Open the folder in VS Code and run index.html via Live Server (Port 5500).


🧠 Reflection (Updated)
🟢 What went well
Core Logic Implementation: Successfully moved from hardcoded values to a dynamic security scoring system in the backend.

Fullstack Integration: The connection between the C# scoring logic and the JavaScript dashboard is seamless.

Security Foundation: Implemented JWT authentication and protected API endpoints using the [Authorize] attribute.

🔴 Challenges
Data Consistency: Ensuring that the SecurityScore and SecurityGrade calculated on the backend correctly map to the frontend display.

Auth Flow: Managing token expiration and handling 401 Unauthorized errors during the development of the scanner.

CORS & Environment: Coordinating communication between the Frontend (Port 5500) and the Backend API (Port 7290).
