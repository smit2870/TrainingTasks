# 📘 Trainee Management API

##  Project Name

taskmanagement(TraineeManagement)

---

##  Technology Used
- C#
- ASP.NET Core Web API
- MySQL
- Swagger (OpenAPI)
- LINQ
- JSON

---

## Database Setup (MySQL)

1. Install MySql Server
```
sudo apt install mysql-server -y
```

2. Start MySql Server
```
sudo service mysql start
```
3. Run the server
```
mysql -u root -p
```

4. Update Connection String
Edit your appsettings.json
```
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;database=TraineeDB;user=root;password=yourpassword"
}
```

5. Install Required Packages
```
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```



##  How to Run

1. Clone the repository
```
git clone https://github.com/smit2870/TrainingTasks.git  
cd taskmanagement
```

2. Run the project
```
dotnet run  
```

3. Open Swagger UI in browser:
```
https://localhost:7071/swagger  
```

---

##  API List

|Method|   Endpoint | Description |
|--------| --------| -------- |
|`GET`| `/api/health`  | Check API Status  |
|`GET`|`/api/Trainee` | Fetch all trainees from db|
|`GET`|`/api/Trainee?={searchQuery}` | Fetch trainees based on searchQuery|
|`GET`|`/api/Trainee/{id}` | Fech a particular trainee by ID |
|`POST`|`/api/Trainee` | Post trainee  |
|`PUT`|`/api/Trainee/{id}` | Update trainee  |
|`DELETE`|`/api/Trainee/{id}` | Delete trainee  |

---

##  Sample Request JSON
```
POST /api/trainees

{
  "firstName": "smit",
  "lastName": "patel",
  "email": "smit123a@gmail.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Available"
}
```
---

##  Sample Response JSON

Success Response for Create and Get All Trainee Details.
```
{
  "id": 1,
  "firstName": "smit",
  "lastName": "patel",
  "email": "smit123@gmail.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Available"
  "createdDate": "2026-06-08T10:45:38.3936817Z",
  "updatedDate": "2026-06-08T10:45:38.3936817Z"
}
```

Success Response for Get Trainee By Id and Changing Details(Put Request).
```
{
  "id": 1
  "firstName": "smit",
  "lastName": "patel",
  "email": "smit123@gmail.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Available"
  "updatedDate": "2026-06-08T10:45:38.3936817Z"
}
```
---


##  Known Limitations

- No authentication or authorization
- Limited validation rules

---

##  Future Improvements

- Implement global error handling