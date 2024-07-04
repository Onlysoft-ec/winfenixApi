using winfenixApi.Application.Interfaces;
using winfenixApi.Core.Interfaces;
using winfenixApi.Core.Validators;

namespace winfenixApi.Application.Services
{
    public class DynamicService : IDynamicService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IDictionary<string, IValidator> _validators;
        private readonly ILogger<DynamicService> _logger;

        public DynamicService(IDynamicRepository dynamicRepository, IDictionary<string, IValidator> validators, ILogger<DynamicService> logger)
        {
            _dynamicRepository = dynamicRepository;
            _validators = validators;
            _logger = logger;
        }

        public Task<IEnumerable<dynamic>> GetAllAsync(string tableName)
        {
            return _dynamicRepository.GetAllAsync(tableName);
        }

        public Task<object?> GetByIdAsync(string tableName, int id)
        {
            return _dynamicRepository.GetByIdAsync(tableName, id);
        }

        public async Task<int> CreateAsync(string tableName, Dictionary<string, object> data)
        {
            if (_validators.ContainsKey(tableName))
            {
                var validationMessage = await _validators[tableName].ValidateCreateAsync(data);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    _logger.LogError($"Validation error for Create on table {tableName}: {validationMessage}");
                    throw new Exception(validationMessage);
                }
            }

            return await _dynamicRepository.CreateAsync(tableName, data);
        }

        public async Task<int> UpdateAsync(string tableName, int id, Dictionary<string, object> data)
        {
            if (_validators.ContainsKey(tableName))
            {
                var validationMessage = await _validators[tableName].ValidateUpdateAsync(data);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    _logger.LogError($"Validation error for Update on table {tableName}: {validationMessage}");
                    throw new Exception(validationMessage);
                }
            }

            return await _dynamicRepository.UpdateAsync(tableName, id, data);
        }

        public async Task<int> DeleteAsync(string tableName, int id)
        {
            if (_validators.ContainsKey(tableName))
            {
                var validationMessage = await _validators[tableName].ValidateDeleteAsync(id);
                if (!string.IsNullOrEmpty(validationMessage))
                {
                    _logger.LogError($"Validation error for Delete on table {tableName}: {validationMessage}");
                    throw new Exception(validationMessage);
                }
            }

            return await _dynamicRepository.DeleteAsync(tableName, id);
        }
    }
}
