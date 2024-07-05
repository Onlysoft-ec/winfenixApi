using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using winfenixApi.WebApi;

namespace winfenixApi.Infrastructure.Data
{
    public class DatabaseConnection
    {
        private readonly IConfiguration _configuration;
        private SqlConnection _connection;
        private string _connectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ConnectAsync(string server, string userDb, string passDb)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection")
                .Replace("{Server}", EncryptionHelper.Decrypt(server))
                .Replace("{UserDb}", EncryptionHelper.Decrypt(userDb))
                .Replace("{PassDb}", EncryptionHelper.Decrypt(passDb));

            _connection = new SqlConnection(connectionString);
            int retryCount = 0;

            while (retryCount < 3)
            {
                try
                {
                    await _connection.OpenAsync();
                    break;
                }
                catch (Exception)
                {
                    retryCount++;
                    if (retryCount == 3)
                    {
                        throw new Exception("Failed to connect to the database after 3 attempts.");
                    }
                }
            }
        }

        public bool IsConnected()
        {
            return _connection != null && _connection.State == System.Data.ConnectionState.Open;
        }

        public SqlConnection GetConnection()
        {
            if (!IsConnected())
            {
                throw new InvalidOperationException("Database connection is not open.");
            }
            return _connection;
        }

        public void CloseConnection()
        {
            _connection?.Close();
        }
    }
}
