namespace winfenixApi.Core.Interfaces
{
    public interface IWeatherForecastService
    {
        Task<IEnumerable<WeatherForecast>> GetForecastsAsync();
    }
}
