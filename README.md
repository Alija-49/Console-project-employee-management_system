# Employee Management System

A simple, complete CRUD console application built with **C#, .NET 8, ADO.NET, and SQL Server**
(MySQL script also included). Designed as a portfolio project demonstrating core C#/.NET data-access
skills without relying on an ORM like Entity Framework.

## Features

- **Add Employee** – create a new employee record
- **View Employees** – list all employees
- **Update Employee** – edit an existing employee's details
- **Delete Employee** – remove an employee (with confirmation)
- **Search Employee** – search by name, email, department, designation, or ID
- **Department Report** – employees grouped by department with average salary (LINQ)

## Concepts Demonstrated

| Concept              | Where |
|-----------------------|-------|
| C# / OOP              | `Models/Employee.cs` — encapsulation, validation, constructors |
| ADO.NET                | `Data/DatabaseHelper.cs`, `Repository/EmployeeRepository.cs` — `SqlConnection`, `SqlCommand`, `SqlDataReader` |
| SQL / CRUD             | Parameterized `INSERT`, `SELECT`, `UPDATE`, `DELETE` statements |
| Exception Handling     | Custom exceptions (`EmployeeNotFoundException`, `DataAccessException`) + try/catch in `Program.cs` |
| Collections            | `List<Employee>`, `Dictionary<string, List<Employee>>` |
| LINQ                   | `Where`, `GroupBy`, `OrderBy`, `Average`, `ToDictionary` in `EmployeeRepository.cs` |

## Project Structure

```
EmployeeManagementSystem/
├── EmployeeManagementSystem.csproj
├── appsettings.json              # connection string lives here
├── Program.cs                    # console menu / entry point
├── Models/
│   ├── Employee.cs                # entity with OOP + validation
│   └── Exceptions.cs              # custom exception types
├── Data/
│   └── DatabaseHelper.cs          # builds SqlConnection from config
├── Repository/
│   └── EmployeeRepository.cs      # all ADO.NET CRUD + LINQ queries
└── Database/
    ├── schema.sql                 # SQL Server table + seed data
    └── schema_mysql.sql           # MySQL equivalent
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server, SQL Server Express, or **SQL Server LocalDB** (installed with Visual Studio)
  - MySQL can be used instead — see the note at the bottom of `Database/schema_mysql.sql`

## Setup & Run

### 1. Create the database

Open the script in SQL Server Management Studio, Azure Data Studio, or `sqlcmd`, and run it:

```
Database/schema.sql
```

This creates the `EmployeeManagementDB` database, the `Employees` table, and inserts 5 sample rows.

If you're using LocalDB, you can also run it from the command line:

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -i Database/schema.sql
```

### 2. Configure the connection string

Open `appsettings.json` and update the connection string if needed:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EmployeeManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

For a full SQL Server instance with SQL authentication, it would look more like:

```
Server=localhost;Database=EmployeeManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;
```

### 3. Restore & run

```bash
cd EmployeeManagementSystem
dotnet restore
dotnet run
```

You'll see a menu-driven console app:

```
=================================================
       EMPLOYEE MANAGEMENT SYSTEM (C# / ADO.NET)
=================================================
-------------------------------------------------
 1. Add Employee
 2. View All Employees
 3. Update Employee
 4. Delete Employee
 5. Search Employee
 6. Department Report (LINQ)
 0. Exit
-------------------------------------------------
Enter your choice:
```

## Notes

- All SQL commands use **parameterized queries** to prevent SQL injection.
- Validation (e.g. required fields, valid email, non-negative salary) is enforced in
  the `Employee` model's property setters, and invalid input raises `ArgumentException`,
  which is caught and displayed cleanly by the console app.
- Database errors are wrapped in a custom `DataAccessException` so the UI layer never
  has to deal with raw `SqlException` details.
- This project intentionally avoids Entity Framework / Dapper so that raw ADO.NET usage
  is front and center for learning purposes. Swapping in EF Core later would be a natural
  "next step" extension of this project.

## Possible Extensions

- Add a WPF or ASP.NET Core Web API front end on top of the same repository layer
- Add pagination for `GetAllEmployees`
- Add role-based authentication (Admin vs read-only)
- Add unit tests for `EmployeeRepository` using a mocked/in-memory database

Sk Alija
