using System.Threading.Tasks;

namespace winfenixApi.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(string username, string password, string server, string database, string dbUser, string dbPassword);
        Task<bool> RegisterAsync(string username, string password, string server, string database, string dbUser, string dbPassword);
        Task<bool> ValidateTokenAsync(string token);
    }
}
