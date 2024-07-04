using winfenixApi.Core.Entities;

namespace winfenixApi.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsernameAndPasswordAsync(string username, string password);
    }
}
