using System;
using Microsoft.Extensions.Configuration;
using EmployeeManagementSystem.Data;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Repository;

namespace EmployeeManagementSystem
{
    public class Program
    {
        private static EmployeeRepository _repository = null!;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var dbHelper = new DatabaseHelper(configuration);
            _repository = new EmployeeRepository(dbHelper);

            Console.WriteLine("=================================================");
            Console.WriteLine("       EMPLOYEE MANAGEMENT SYSTEM (C# / ADO.NET)");
            Console.WriteLine("=================================================");

            if (!dbHelper.TestConnection())
            {
                Console.WriteLine();
                Console.WriteLine("!! Could not connect to the database.");
                Console.WriteLine("   Make sure SQL Server / LocalDB is running and that you have");
                Console.WriteLine("   executed Database/schema.sql, then update appsettings.json");
                Console.WriteLine("   with the correct connection string. See README.md for help.");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            RunMenuLoop();
        }

        private static void RunMenuLoop()
        {
            bool exit = false;

            while (!exit)
            {
                PrintMenu();
                string? choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            AddEmployeeFlow();
                            break;
                        case "2":
                            ViewAllEmployeesFlow();
                            break;
                        case "3":
                            UpdateEmployeeFlow();
                            break;
                        case "4":
                            DeleteEmployeeFlow();
                            break;
                        case "5":
                            SearchEmployeeFlow();
                            break;
                        case "6":
                            DepartmentReportFlow();
                            break;
                        case "0":
                            exit = true;
                            Console.WriteLine("Goodbye!");
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please select a valid menu option.");
                            break;
                    }
                }
                catch (DataAccessException ex)
                {
                    Console.WriteLine($"[DATABASE ERROR] {ex.Message}");
                    Console.WriteLine($"   Details: {ex.InnerException?.Message}");
                }
                catch (EmployeeNotFoundException ex)
                {
                    Console.WriteLine($"[NOT FOUND] {ex.Message}");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"[VALIDATION ERROR] {ex.Message}");
                }
                catch (FormatException)
                {
                    Console.WriteLine("[INPUT ERROR] The value you entered was not in the expected format.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[UNEXPECTED ERROR] {ex.Message}");
                }

                if (!exit)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press any key to return to the menu...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
        }

        private static void PrintMenu()
        {
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine(" 1. Add Employee");
            Console.WriteLine(" 2. View All Employees");
            Console.WriteLine(" 3. Update Employee");
            Console.WriteLine(" 4. Delete Employee");
            Console.WriteLine(" 5. Search Employee");
            Console.WriteLine(" 6. Department Report (LINQ)");
            Console.WriteLine(" 0. Exit");
            Console.WriteLine("-------------------------------------------------");
            Console.Write("Enter your choice: ");
        }

        // -------------------------------------------------------------
        // Menu flows
        // -------------------------------------------------------------
        private static void AddEmployeeFlow()
        {
            Console.WriteLine("--- Add New Employee ---");

            Console.Write("First Name: ");
            string firstName = Console.ReadLine() ?? string.Empty;

            Console.Write("Last Name: ");
            string lastName = Console.ReadLine() ?? string.Empty;

            Console.Write("Email: ");
            string email = Console.ReadLine() ?? string.Empty;

            Console.Write("Phone Number (optional): ");
            string phone = Console.ReadLine() ?? string.Empty;

            Console.Write("Department: ");
            string department = Console.ReadLine() ?? string.Empty;

            Console.Write("Designation: ");
            string designation = Console.ReadLine() ?? string.Empty;

            Console.Write("Salary: ");
            decimal salary = decimal.Parse(Console.ReadLine() ?? "0");

            Console.Write("Date of Joining (yyyy-MM-dd): ");
            DateTime doj = DateTime.Parse(Console.ReadLine() ?? DateTime.Today.ToString("yyyy-MM-dd"));

            var employee = new Employee(
                firstName, lastName, email,
                string.IsNullOrWhiteSpace(phone) ? null : phone,
                department, designation, salary, doj);

            int newId = _repository.AddEmployee(employee);
            Console.WriteLine($"\nEmployee added successfully with ID: {newId}");
        }

        private static void ViewAllEmployeesFlow()
        {
            Console.WriteLine("--- All Employees ---");
            var employees = _repository.GetAllEmployees();

            if (employees.Count == 0)
            {
                Console.WriteLine("No employees found.");
                return;
            }

            foreach (var emp in employees)
            {
                Console.WriteLine(emp);
            }
            Console.WriteLine($"\nTotal employees: {employees.Count}");
        }

        private static void UpdateEmployeeFlow()
        {
            Console.WriteLine("--- Update Employee ---");
            Console.Write("Enter Employee ID to update: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            var existing = _repository.GetEmployeeById(id)
                ?? throw new EmployeeNotFoundException(id);

            Console.WriteLine($"Current details: {existing}");
            Console.WriteLine("Press Enter to keep the current value for any field.\n");

            Console.Write($"First Name [{existing.FirstName}]: ");
            string input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input)) existing.FirstName = input;

            Console.Write($"Last Name [{existing.LastName}]: ");
            input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input)) existing.LastName = input;

            Console.Write($"Email [{existing.Email}]: ");
            input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input)) existing.Email = input;

            Console.Write($"Phone Number [{existing.PhoneNumber}]: ");
            input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input)) existing.PhoneNumber = input;

            Console.Write($"Department [{existing.Department}]: ");
            input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input)) existing.Department = input;

            Console.Write($"Designation [{existing.Designation}]: ");
            input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input)) existing.Designation = input;

            Console.Write($"Salary [{existing.Salary}]: ");
            input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input)) existing.Salary = decimal.Parse(input);

            Console.Write($"Date of Joining [{existing.DateOfJoining:yyyy-MM-dd}]: ");
            input = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(input)) existing.DateOfJoining = DateTime.Parse(input);

            bool updated = _repository.UpdateEmployee(existing);
            Console.WriteLine(updated
                ? "\nEmployee updated successfully."
                : "\nUpdate failed — no rows were affected.");
        }

        private static void DeleteEmployeeFlow()
        {
            Console.WriteLine("--- Delete Employee ---");
            Console.Write("Enter Employee ID to delete: ");
            int id = int.Parse(Console.ReadLine() ?? "0");

            var existing = _repository.GetEmployeeById(id)
                ?? throw new EmployeeNotFoundException(id);

            Console.Write($"Are you sure you want to delete '{existing.FullName}'? (y/n): ");
            string confirm = Console.ReadLine() ?? "n";

            if (confirm.Trim().ToLower() == "y")
            {
                bool deleted = _repository.DeleteEmployee(id);
                Console.WriteLine(deleted
                    ? "Employee deleted successfully."
                    : "Delete failed — no rows were affected.");
            }
            else
            {
                Console.WriteLine("Delete cancelled.");
            }
        }

        private static void SearchEmployeeFlow()
        {
            Console.WriteLine("--- Search Employee ---");
            Console.Write("Enter search term (name, email, department, designation, or ID): ");
            string term = Console.ReadLine() ?? string.Empty;

            var results = _repository.SearchEmployees(term);

            if (results.Count == 0)
            {
                Console.WriteLine("No matching employees found.");
                return;
            }

            foreach (var emp in results)
            {
                Console.WriteLine(emp);
            }
            Console.WriteLine($"\n{results.Count} matching employee(s) found.");
        }

        private static void DepartmentReportFlow()
        {
            Console.WriteLine("--- Department Report (grouped using LINQ) ---");
            var grouped = _repository.GetEmployeesByDepartment();
            var avgSalaries = _repository.GetAverageSalaryByDepartment();

            if (grouped.Count == 0)
            {
                Console.WriteLine("No employees found.");
                return;
            }

            foreach (var kvp in grouped)
            {
                Console.WriteLine($"\nDepartment: {kvp.Key}  (Average Salary: ${avgSalaries[kvp.Key]:N2})");
                foreach (var emp in kvp.Value)
                {
                    Console.WriteLine($"   - {emp.FullName} | {emp.Designation} | ${emp.Salary:N2}");
                }
            }
        }
    }
}
