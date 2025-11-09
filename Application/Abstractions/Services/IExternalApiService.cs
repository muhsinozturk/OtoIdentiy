using Application.DTOs.External;

namespace Application.Abstractions.Services
{
    public interface IExternalApiService
    {
        Task<CurrencyDto> GetCurrencyRatesAsync();
        Task<WeatherDto> GetWeatherAsync(string city);
        Task<GoldDto> GetGoldPricesAsync();
    }
}
