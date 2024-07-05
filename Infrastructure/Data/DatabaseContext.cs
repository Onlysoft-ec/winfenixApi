using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;

namespace winfenixApi.Infrastructure.Data
{
    public class DatabaseContext
    {
        private string? _connectionString;
        //private readonly DatabaseSettings _databaseSettings;

        public DatabaseContext()
        {
            var json = File.ReadAllText("SqlConnect.json");
            var config = JObject.Parse(json);

            string? server = config["Server"]?.ToString();
            string? user = config["UserDb"]?.ToString();
            string? password = config["PassDb"]?.ToString();

            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("Invalid database connection settings in SqlConnect.json.");
            }

            _connectionString = $"Server={server};User Id={user};Password={password};";
        }

        public void SetConnectionString(string server, string user, string password)
        {
            if (string.IsNullOrEmpty(server) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Invalid database connection parameters.");
            }

            _connectionString = $"Server={server};User Id={user};Password={password};";
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // Agregar método para crear conexión con un string de conexión personalizado
        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
