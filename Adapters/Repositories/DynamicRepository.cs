using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using winfenixApi.Core.Interfaces;
using winfenixApi.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace winfenixApi.Repositories
{
    public class DynamicRepository : IDynamicRepository
    {
        private readonly DatabaseContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<DynamicRepository> _logger;

        public DynamicRepository(DatabaseContext context, IHttpContextAccessor httpContextAccessor, ILogger<DynamicRepository> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private string GetConnectionString()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                throw new InvalidOperationException("No active HttpContext.");
            }

            if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                throw new InvalidOperationException("Authorization header is missing.");
            }

            var accessToken = authorizationHeader.ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(accessToken) as JwtSecurityToken;

            if (jsonToken == null)
            {
                throw new InvalidOperationException("Invalid token.");
            }

            var server = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "Server")?.Value
                         ?? throw new InvalidOperationException("Server claim is missing.");
            var database = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "Database")?.Value
                           ?? throw new InvalidOperationException("Database claim is missing.");
            var dbUser = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "DbUser")?.Value
                         ?? throw new InvalidOperationException("DbUser claim is missing.");
            var dbPassword = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "DbPassword")?.Value
                             ?? throw new InvalidOperationException("DbPassword claim is missing.");


            return $"Server={server};Database={database};User Id={dbUser};Password={dbPassword};";
        }

        private IDbConnection CreateConnection()
        {
            var connectionString = GetConnectionString();
            return _context.CreateConnection(connectionString);
        }

        private string FormatValue(object value)
        {
            if (value == null)
            {
                return "NULL";
            }
            else if (value is string str)
            {
                return $"'{str.Replace("'", "''")}'"; // Escape single quotes
            }
            else if (value is char chr)
            {
                return $"'{chr}'"; // Escape single quotes
            }
            else if (value is bool boolean)
            {
                return boolean ? "1" : "0";
            }
            else if (value is DateTime dateTime)
            {
                return $"'{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}'";
            }
            else
            {
                return value?.ToString() ?? "NULL";
            }
        }

        private async Task<IEnumerable<string>> GetIdentityColumnsAsync(string tableName)
        {
            using var connection = CreateConnection();
            {
                var query = @"
                    SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = @TableName AND COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1";

                return await connection.QueryAsync<string>(query, new { TableName = tableName });
            }
        }

        public async Task<IEnumerable<dynamic>> GetAllAsync(string tableName)
        {
            {
                using var connection = CreateConnection();
                var query = $"SELECT * FROM {tableName}";
                return await connection.QueryAsync<dynamic>(query);
            }
        }

        public async Task<object?> GetByIdAsync(string tableName, int id)
        {
            {
                using var connection = CreateConnection();
                var query = $"SELECT * FROM {tableName} WHERE Id = @Id";
                var result = await connection.QuerySingleOrDefaultAsync<object>(query, new { Id = id });
                return result ?? null;
            }
        }

        public async Task<int> CreateAsync(string tableName, Dictionary<string, object> data)
        {
            var identityColumns = await GetIdentityColumnsAsync(tableName);
            var filteredData = data.Where(d => !identityColumns.Contains(d.Key)).ToDictionary(d => d.Key, d => d.Value);

            {
                using var connection = CreateConnection();
                var columns = string.Join(", ", filteredData.Keys);
                var values = string.Join(", ", filteredData.Values.Select(v => FormatValue(v)));
                var query = $"INSERT INTO {tableName} ({columns}) VALUES ({values}); SELECT CAST(SCOPE_IDENTITY() as int)";
                return await connection.ExecuteScalarAsync<int>(query);
            }
        }

        public async Task<int> UpdateAsync(string tableName, int id, Dictionary<string, object> data)
        {
            var identityColumns = await GetIdentityColumnsAsync(tableName);
            var filteredData = data.Where(d => !identityColumns.Contains(d.Key)).ToDictionary(d => d.Key, d => d.Value);
                        
            {
                using var connection = CreateConnection();
                var setClause = string.Join(", ", filteredData.Keys.Select(k => $"{k} = {FormatValue(filteredData[k])}"));
                var query = $"UPDATE {tableName} SET {setClause} WHERE Id = @Id";
                return await connection.ExecuteAsync(query, new { Id = id });
            }
        }

        public async Task<int> DeleteAsync(string tableName, int id)
        {
            {
                using var connection = CreateConnection();
                var query = $"DELETE FROM {tableName} WHERE Id = @Id";
                return await connection.ExecuteAsync(query, new { Id = id });
            }
        }
    }
}
