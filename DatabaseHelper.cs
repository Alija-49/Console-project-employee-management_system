using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EmployeeManagementSystem.Data
{
    /// <summary>
    /// Centralizes access to the connection string and creation of
    /// ADO.NET SqlConnection objects.
    /// </summary>
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found in appsettings.json.");
        }

        public SqlConnection GetConnection() => new SqlConnection(_connectionString);

        /// <summary>
        /// Verifies the database is reachable and the Employees table exists.
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
