using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Options;
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

        public IDbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
