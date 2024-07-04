using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using winfenixApi.Core.Interfaces;
using winfenixApi.Core.Entities;
using Dapper;
using System.Data.SqlClient;

namespace winfenixApi.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<string?> LoginAsync(string username, string password, string server, string database, string dbUser, string dbPassword)
        {
            var user = await _userRepository.GetUserByUsernameAndPasswordAsync(username, password);

            if (user == null)
                return null;

            return GenerateJwtToken(user);
        }

        public async Task<bool> RegisterAsync(string username, string password, string server, string database, string dbUser, string dbPassword)
        {
            var connectionString = $"Server={server};Database={database};User Id={dbUser};Password={dbPassword};";

            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT COUNT(1) FROM Usuarios WHERE Username = @Username";
                var userExists = await connection.ExecuteScalarAsync<bool>(query, new { Username = username });

                if (userExists)
                {
                    return false; // Usuario ya existe
                }

                var insertQuery = "INSERT INTO Usuarios (Username, Password) VALUES (@Username, @Password)";
                var result = await connection.ExecuteAsync(insertQuery, new { Username = username, Password = password });

                return result > 0;
            }
        }


        public Task<bool> ValidateTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }

            var keyBytes = Encoding.ASCII.GetBytes(key);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // Set ClockSkew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["Jwt:Key"];

            if (string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }

            var keyBytes = Encoding.ASCII.GetBytes(key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
