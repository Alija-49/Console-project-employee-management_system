using System;

namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// Represents an Employee entity.
    /// Demonstrates encapsulation (private fields + public properties)
    /// and basic validation logic within the object itself.
    /// </summary>
    public class Employee
    {
        public int EmployeeId { get; set; }

        private string _firstName = string.Empty;
        public string FirstName
        {
            get => _firstName;
            set => _firstName = !string.IsNullOrWhiteSpace(value)
                ? value.Trim()
                : throw new ArgumentException("First name cannot be empty.");
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get => _lastName;
            set => _lastName = !string.IsNullOrWhiteSpace(value)
                ? value.Trim()
                : throw new ArgumentException("Last name cannot be empty.");
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => _email = IsValidEmail(value)
                ? value.Trim()
                : throw new ArgumentException("A valid email address is required.");
        }

        public string? PhoneNumber { get; set; }

        public string Department { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        private decimal _salary;
        public decimal Salary
        {
            get => _salary;
            set => _salary = value >= 0
                ? value
                : throw new ArgumentException("Salary cannot be negative.");
        }

        public DateTime DateOfJoining { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// Full name convenience property (computed, not stored)
        public string FullName => $"{FirstName} {LastName}";

        public Employee() { }

        public Employee(string firstName, string lastName, string email, string? phoneNumber,
            string department, string designation, decimal salary, DateTime dateOfJoining)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Department = department;
            Designation = designation;
            Salary = salary;
            DateOfJoining = dateOfJoining;
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email.Trim();
            }
            catch
            {
                return false;
            }
        }

        public override string ToString()
        {
            return $"[{EmployeeId}] {FullName,-25} {Department,-15} {Designation,-20} " +
                   $"${Salary,10:N2}  Joined: {DateOfJoining:yyyy-MM-dd}  Email: {Email}";
        }
    }
}
