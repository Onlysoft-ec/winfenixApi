using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;
using winfenixApi.Infrastructure.Configurations;

namespace winfenixApi.Infrastructure.Data
{
    public class DatabaseContext
    {
        private readonly DatabaseSettings _databaseSettings;

        public DatabaseContext(IOptions<DatabaseSettings> databaseSettings)
        {
            _databaseSettings = databaseSettings.Value;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_databaseSettings.ConnectionString);
        }

        // Agregar método para crear conexión con un string de conexión personalizado
        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
