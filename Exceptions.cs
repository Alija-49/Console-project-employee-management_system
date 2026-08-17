using System;

namespace EmployeeManagementSystem.Models
{
    /// <summary>
    /// Thrown when an operation references an employee that does not exist.
    /// </summary>
    public class EmployeeNotFoundException : Exception
    {
        public EmployeeNotFoundException(int employeeId)
            : base($"Employee with ID {employeeId} was not found.") { }

        public EmployeeNotFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Thrown when a database operation fails, wrapping the underlying
    /// ADO.NET exception with a friendlier message.
    /// </summary>
    public class DataAccessException : Exception
    {
        public DataAccessException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
