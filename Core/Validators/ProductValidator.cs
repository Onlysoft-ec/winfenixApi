namespace winfenixApi.Core.Validators
{
    public class ProductValidator : IValidator
    {
        public Task<string?> ValidateCreateAsync(Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(data["Nombre"]?.ToString()))
            {
                return Task.FromResult<string?>("El nombre del producto no puede estar vacío.");
            }

            if (data["Precio"] == null || (decimal)data["Precio"] <= 0)
            {
                return Task.FromResult<string?>("El precio del producto debe ser mayor que cero.");
            }

            return Task.FromResult<string?>(null);
        }

        public Task<string?> ValidateUpdateAsync(Dictionary<string, object> data)
        {
            return ValidateCreateAsync(data);
        }

        public Task<string?> ValidateDeleteAsync(int id)
        {
            return Task.FromResult<string?>(null);
        }
    }
}
