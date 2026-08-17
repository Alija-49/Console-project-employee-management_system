-- ============================================================
-- Employee Management System - Database Setup Script
-- Works on SQL Server / SQL Server LocalDB
-- ============================================================

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'EmployeeManagementDB')
BEGIN
    CREATE DATABASE EmployeeManagementDB;
END
GO

USE EmployeeManagementDB;
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Employees')
BEGIN
    CREATE TABLE Employees (
        EmployeeId      INT IDENTITY(1,1) PRIMARY KEY,
        FirstName       NVARCHAR(50)  NOT NULL,
        LastName        NVARCHAR(50)  NOT NULL,
        Email           NVARCHAR(100) NOT NULL UNIQUE,
        PhoneNumber     NVARCHAR(20)  NULL,
        Department      NVARCHAR(50)  NOT NULL,
        Designation     NVARCHAR(50)  NOT NULL,
        Salary          DECIMAL(12,2) NOT NULL,
        DateOfJoining   DATE          NOT NULL,
        CreatedAt       DATETIME      NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Sample seed data (optional)
IF NOT EXISTS (SELECT 1 FROM Employees)
BEGIN
    INSERT INTO Employees (FirstName, LastName, Email, PhoneNumber, Department, Designation, Salary, DateOfJoining)
    VALUES
    ('John', 'Smith', 'john.smith@company.com', '555-0101', 'Engineering', 'Software Engineer', 75000.00, '2022-03-15'),
    ('Priya', 'Sharma', 'priya.sharma@company.com', '555-0102', 'Human Resources', 'HR Manager', 68000.00, '2021-06-01'),
    ('Michael', 'Chen', 'michael.chen@company.com', '555-0103', 'Finance', 'Financial Analyst', 62000.00, '2023-01-10'),
    ('Sarah', 'Johnson', 'sarah.johnson@company.com', '555-0104', 'Engineering', 'Senior Developer', 92000.00, '2019-09-23'),
    ('Ravi', 'Kumar', 'ravi.kumar@company.com', '555-0105', 'Sales', 'Sales Executive', 55000.00, '2022-11-05');
END
GO
