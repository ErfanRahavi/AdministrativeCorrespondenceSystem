# Administrative Correspondence System

This project is an **ASP.NET Core Web API** designed to manage organizational correspondence effectively. It provides functionalities to handle letters, manage users, define letter types, and perform detailed reporting. 

## **Technologies Used**
- **Framework**: .NET 8
- **Database**: SQL Server
- **IDE**: Visual Studio 2022
- **ORM**: Entity Framework Core
- **Language**: C#
- **Tools**: Swagger for API documentation

## **Features**
### **1. Letter Management**
- Create, reply, forward, and delete letters.
- Attach files to letters.
- Define letter types (normal, confidential, urgent).

### **2. User Management**
- Manage sender and receiver information for each letter.

### **3. Reporting**
- Generate reports based on filters like:
  - Sender ID
  - Receiver ID
  - Date range
  - Subject
- Paginated results for efficient data retrieval.

### **4. File Attachments**
- Upload and manage file attachments for letters.
- Delete files when letters are deleted or updated.

---

## **Installation and Setup**

### **Prerequisites**
1. **Install .NET 8 SDK**  
   Download and install the [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).  
2. **Install SQL Server**  
   Ensure SQL Server is installed and running locally or on a server.  
3. **Install Visual Studio 2022**  
   Install Visual Studio 2022 with the following workloads:
   - ASP.NET and web development
   - .NET desktop development

---

### **Steps**
1. Clone the repository:  
   ```bash
   git clone https://github.com/ErfanRahavi/AdministrativeCorrespondenceSystem.git
   cd AdministrativeCorrespondenceSystem
