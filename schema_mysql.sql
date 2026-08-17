-- ============================================================
-- Employee Management System - Database Setup Script (MySQL)
-- ============================================================

CREATE DATABASE IF NOT EXISTS EmployeeManagementDB;
USE EmployeeManagementDB;

CREATE TABLE IF NOT EXISTS Employees (
    EmployeeId      INT AUTO_INCREMENT PRIMARY KEY,
    FirstName       VARCHAR(50)  NOT NULL,
    LastName        VARCHAR(50)  NOT NULL,
    Email           VARCHAR(100) NOT NULL UNIQUE,
    PhoneNumber     VARCHAR(20)  NULL,
    Department      VARCHAR(50)  NOT NULL,
    Designation     VARCHAR(50)  NOT NULL,
    Salary          DECIMAL(12,2) NOT NULL,
    DateOfJoining   DATE          NOT NULL,
    CreatedAt       DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP
);

INSERT INTO Employees (FirstName, LastName, Email, PhoneNumber, Department, Designation, Salary, DateOfJoining)
SELECT * FROM (
    SELECT 'John' AS FirstName, 'Smith' AS LastName, 'john.smith@company.com' AS Email, '555-0101' AS PhoneNumber, 'Engineering' AS Department, 'Software Engineer' AS Designation, 75000.00 AS Salary, '2022-03-15' AS DateOfJoining
    UNION ALL SELECT 'Priya', 'Sharma', 'priya.sharma@company.com', '555-0102', 'Human Resources', 'HR Manager', 68000.00, '2021-06-01'
    UNION ALL SELECT 'Michael', 'Chen', 'michael.chen@company.com', '555-0103', 'Finance', 'Financial Analyst', 62000.00, '2023-01-10'
    UNION ALL SELECT 'Sarah', 'Johnson', 'sarah.johnson@company.com', '555-0104', 'Engineering', 'Senior Developer', 92000.00, '2019-09-23'
    UNION ALL SELECT 'Ravi', 'Kumar', 'ravi.kumar@company.com', '555-0105', 'Sales', 'Sales Executive', 55000.00, '2022-11-05'
) AS seed
WHERE NOT EXISTS (SELECT 1 FROM Employees);

-- NOTE: To use MySQL instead of SQL Server with this project:
--   1. Replace the Microsoft.Data.SqlClient package reference in the .csproj
--      with MySql.Data (or MySqlConnector).
--   2. Replace SqlConnection/SqlCommand/SqlDataReader with the MySql equivalents
--      (MySqlConnection/MySqlCommand/MySqlDataReader) in Data/DatabaseHelper.cs
--      and Repository/EmployeeRepository.cs.
--   3. Update the connection string in appsettings.json, e.g.:
--      "Server=localhost;Database=EmployeeManagementDB;Uid=root;Pwd=yourpassword;"
