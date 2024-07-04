namespace winfenixApi.Core.Interfaces
{
    public interface IDynamicRepository
    {
        Task<IEnumerable<dynamic>> GetAllAsync(string tableName);
        Task<object?> GetByIdAsync(string tableName, int id);
        Task<int> CreateAsync(string tableName, Dictionary<string, object> data);
        Task<int> UpdateAsync(string tableName, int id, Dictionary<string, object> data);
        Task<int> DeleteAsync(string tableName, int id);
    }
}
