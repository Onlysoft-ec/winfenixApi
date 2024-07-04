namespace winfenixApi.Core.Validators
{
    public interface IValidator
    {
        Task<string?> ValidateCreateAsync(Dictionary<string, object> data);
        Task<string?> ValidateUpdateAsync(Dictionary<string, object> data);
        Task<string?> ValidateDeleteAsync(int id);
    }
}
