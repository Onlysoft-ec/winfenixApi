using System.Threading.Tasks;
using winfenixApi.Core.Entities;

namespace winfenixApi.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> ValidateTokenAsync(string token);
        Task<string?> GenerateJwtTokenAsync(string username, string password);
    }
}
