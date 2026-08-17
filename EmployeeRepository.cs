using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;

namespace EmployeeManagementSystem.Repository
{
    /// <summary>
    /// Handles all data-access operations for Employee records using raw ADO.NET
    /// (SqlConnection / SqlCommand / SqlDataReader) rather than an ORM, per the
    /// project's learning goal of demonstrating ADO.NET directly.
    /// </summary>
    public class EmployeeRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public EmployeeRepository(DatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // ---------------------------------------------------------------
        // CREATE
        // ---------------------------------------------------------------
        public int AddEmployee(Employee employee)
        {
            const string query = @"
                INSERT INTO Employees
                    (FirstName, LastName, Email, PhoneNumber, Department, Designation, Salary, DateOfJoining)
                OUTPUT INSERTED.EmployeeId
                VALUES
                    (@FirstName, @LastName, @Email, @PhoneNumber, @Department, @Designation, @Salary, @DateOfJoining);";

            try
            {
                using var connection = _dbHelper.GetConnection();
                using var command = new SqlCommand(query, connection);
                AddEmployeeParameters(command, employee);

                connection.Open();
                var newId = (int)command.ExecuteScalar();
                return newId;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to add employee to the database.", ex);
            }
        }

        // ---------------------------------------------------------------
        // READ (all)
        // ---------------------------------------------------------------
        public List<Employee> GetAllEmployees()
        {
            const string query = "SELECT * FROM Employees ORDER BY EmployeeId;";
            var employees = new List<Employee>();

            try
            {
                using var connection = _dbHelper.GetConnection();
                using var command = new SqlCommand(query, connection);

                connection.Open();
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    employees.Add(MapReaderToEmployee(reader));
                }
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Failed to retrieve employees from the database.", ex);
            }

            return employees;
        }

        // ---------------------------------------------------------------
        // READ (single, by id)
        // ---------------------------------------------------------------
        public Employee? GetEmployeeById(int employeeId)
        {
            const string query = "SELECT * FROM Employees WHERE EmployeeId = @EmployeeId;";

            try
            {
                using var connection = _dbHelper.GetConnection();
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeId", employeeId);

                connection.Open();
                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return MapReaderToEmployee(reader);
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException($"Failed to retrieve employee with ID {employeeId}.", ex);
            }
        }

        // ---------------------------------------------------------------
        // UPDATE
        // ---------------------------------------------------------------
        public bool UpdateEmployee(Employee employee)
        {
            const string query = @"
                UPDATE Employees
                SET FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    PhoneNumber = @PhoneNumber,
                    Department = @Department,
                    Designation = @Designation,
                    Salary = @Salary,
                    DateOfJoining = @DateOfJoining
                WHERE EmployeeId = @EmployeeId;";

            try
            {
                using var connection = _dbHelper.GetConnection();
                using var command = new SqlCommand(query, connection);
                AddEmployeeParameters(command, employee);
                command.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException($"Failed to update employee with ID {employee.EmployeeId}.", ex);
            }
        }

        // ---------------------------------------------------------------
        // DELETE
        // ---------------------------------------------------------------
        public bool DeleteEmployee(int employeeId)
        {
            const string query = "DELETE FROM Employees WHERE EmployeeId = @EmployeeId;";

            try
            {
                using var connection = _dbHelper.GetConnection();
                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmployeeId", employeeId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                throw new DataAccessException($"Failed to delete employee with ID {employeeId}.", ex);
            }
        }

        // ---------------------------------------------------------------
        // SEARCH — fetches candidate rows from SQL, then uses LINQ
        // in-memory to demonstrate filtering/query composition.
        // ---------------------------------------------------------------
        public List<Employee> SearchEmployees(string searchTerm)
        {
            var all = GetAllEmployees();

            if (string.IsNullOrWhiteSpace(searchTerm))
                return all;

            string term = searchTerm.Trim().ToLowerInvariant();

            return all.Where(e =>
                    e.FirstName.ToLowerInvariant().Contains(term) ||
                    e.LastName.ToLowerInvariant().Contains(term) ||
                    e.Email.ToLowerInvariant().Contains(term) ||
                    e.Department.ToLowerInvariant().Contains(term) ||
                    e.Designation.ToLowerInvariant().Contains(term) ||
                    e.EmployeeId.ToString() == term)
                .OrderBy(e => e.LastName)
                .ToList();
        }

        // LINQ-based helper: employees grouped by department (used by the "reports" menu option)
        public Dictionary<string, List<Employee>> GetEmployeesByDepartment()
        {
            var all = GetAllEmployees();
            return all
                .GroupBy(e => e.Department)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.OrderBy(e => e.LastName).ToList());
        }

        // LINQ-based helper: average salary by department
        public Dictionary<string, decimal> GetAverageSalaryByDepartment()
        {
            var all = GetAllEmployees();
            return all
                .GroupBy(e => e.Department)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => Math.Round(g.Average(e => e.Salary), 2));
        }

        // ---------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------
        private static void AddEmployeeParameters(SqlCommand command, Employee employee)
        {
            command.Parameters.AddWithValue("@FirstName", employee.FirstName);
            command.Parameters.AddWithValue("@LastName", employee.LastName);
            command.Parameters.AddWithValue("@Email", employee.Email);
            command.Parameters.AddWithValue("@PhoneNumber", (object?)employee.PhoneNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Department", employee.Department);
            command.Parameters.AddWithValue("@Designation", employee.Designation);
            command.Parameters.AddWithValue("@Salary", employee.Salary);
            command.Parameters.AddWithValue("@DateOfJoining", employee.DateOfJoining);
        }

        private static Employee MapReaderToEmployee(SqlDataReader reader)
        {
            return new Employee
            {
                EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                PhoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("PhoneNumber")),
                Department = reader.GetString(reader.GetOrdinal("Department")),
                Designation = reader.GetString(reader.GetOrdinal("Designation")),
                Salary = reader.GetDecimal(reader.GetOrdinal("Salary")),
                DateOfJoining = reader.GetDateTime(reader.GetOrdinal("DateOfJoining")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
            };
        }
    }
}
