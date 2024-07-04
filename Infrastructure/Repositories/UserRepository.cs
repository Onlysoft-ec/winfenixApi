using Dapper;
using System.Data.SqlClient;
using winfenixApi.Core.Entities;
using winfenixApi.Core.Interfaces;

namespace winfenixApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<User?> GetUserByUsernameAndPasswordAsync(string username, string password)
        {
            var server = _configuration["DatabaseSettings:Server"];
            var database = _configuration["DatabaseSettings:Database"];
            var dbUser = _configuration["DatabaseSettings:User"];
            var dbPassword = _configuration["DatabaseSettings:Password"];

            var connectionString = $"Server={server};Database={database};User Id={dbUser};Password={dbPassword};";
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                return await connection.QuerySingleOrDefaultAsync<User>(query, new { Username = username, Password = password });
            }
        }
    }
}
