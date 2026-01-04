# ExamEdu - Smart Exam Management System
<div align="center">
<img src="https://i.ibb.co/jH3WV8c/Exam-Edu-Logo.png" alt="ExamEdu Logo" width="20%"/>
</div>

An online examination platform with AI-powered cheating detection. Built with ASP.NET Core for schools and universities to conduct secure, monitored exams with automatic grading and real-time proctoring.

## ✨ Technologies

* `ASP.NET Core 5.0`
* `PostgreSQL`
* `SignalR`
* `Redis`
* `Entity Framework Core`
* `JWT Authentication`

## 🚀 Features

Here's what you can do with ExamEdu:

* **AI-Powered Cheating Detection**: The system monitors students in real-time during exams. It automatically detects suspicious behaviour from students' webcams based on their headpose. Teachers get instant alerts when the AI flags potential cheating.

* **Smart Question Banks**: Organize questions by subject, difficulty level, and type. Questions go through an approval workflow to ensure quality before being added to the bank.

* **Auto-Generate Exams**: Create multiple exam versions instantly. Just set the difficulty levels and number of questions, the system does the rest. Each student gets a different version to prevent copying.

* **Instant Grading**: Multiple-choice questions are graded automatically the moment students submit. Teachers can focus on grading essay questions while the system handles the rest.

* **Live Monitoring**: Watch all students taking exams in real-time. See who's online, who disconnected, and get AI alerts for suspicious activity, all from one dashboard.

* **Smart Reports**: Export detailed results to Excel with charts showing class performance, individual progress, and question difficulty analysis.

##  Architecture

### System Architecture
<img width="50%" height="50%" alt="system-architecture-diagram" src="https://github.com/user-attachments/assets/2790d338-ad8b-46ea-ab77-658bb475777a" />


### Design Patterns & Principles

| Pattern | Implementation |
|---------|---------------|
| **Repository Pattern** | Service layer abstracts data access |
| **DTO Pattern** | Clean separation between entities and API |
| **Dependency Injection** | Constructor injection throughout |
| **Middleware Pipeline** | Custom exception handling & CORS |

### Package Diagram
<img width="50%" height="50%" alt="package-diagram" src="https://github.com/user-attachments/assets/4ef58672-c95a-4dcf-a01d-d7504c978df4" />


## 💭 How Can It Be Improved?

- Add facial recognition during exams for enhanced identity verification
- Implement video recording during exams with playback for review
- Add more AI detection patterns (copy-paste behavior, screen capture attempts)
- Create a mobile app for iOS and Android
- Implement advanced analytics with machine learning for predicting student performance
- Add integration with popular learning management systems (Moodle, Canvas)

## 🚦 Running the Project

To run the project in your local environment, follow these steps:

1. Clone the repository to your local machine
2. Make sure you have .NET 5.0, PostgreSQL, and Redis installed
3. Create `appsettings.json` in the root directory with your database and email configuration
4. Run `dotnet restore` to install dependencies
5. Run `dotnet ef database update` to create the database
6. Run `dotnet run` to start the application
7. Open `https://localhost:5001` in your browser
8. Use the Swagger UI at `https://localhost:5001/swagger` to explore the API
