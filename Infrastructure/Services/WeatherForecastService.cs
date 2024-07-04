using Dapper;
using winfenixApi.Core.Interfaces;
using winfenixApi.Infrastructure.Data;

namespace winfenixApi.Infrastructure.Services
{
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly DatabaseContext _dbContext;

        public WeatherForecastService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<WeatherForecast>> GetForecastsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            string query = "SELECT * FROM WeatherForecast";
            return await connection.QueryAsync<WeatherForecast>(query);
        }
    }
}
