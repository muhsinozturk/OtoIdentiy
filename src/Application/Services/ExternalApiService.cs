using Application.Abstractions.Services;
using Application.DTOs.External;
using System.Text.Json;

namespace Application.Services
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CurrencyDto> GetCurrencyRatesAsync()
        {
            var dto = new CurrencyDto();
            try
            {
                var response = await _httpClient.GetStringAsync("https://open.er-api.com/v6/latest/TRY");
                using var json = JsonDocument.Parse(response);

                if (json.RootElement.TryGetProperty("rates", out var rates))
                {
                    if (rates.TryGetProperty("USD", out var usdProp))
                        dto.USD = Math.Round(1 / usdProp.GetDecimal(), 2);

                    if (rates.TryGetProperty("EUR", out var eurProp))
                        dto.EUR = Math.Round(1 / eurProp.GetDecimal(), 2);
                }
            }
            catch { dto.USD = dto.EUR = 0; }
            return dto;
        }

        public async Task<WeatherDto> GetWeatherAsync(string city)
        {
            var dto = new WeatherDto { City = city };
            try
            {
                var response = await _httpClient.GetStringAsync($"https://wttr.in/{city}?format=j1");
                using var json = JsonDocument.Parse(response);
                var cond = json.RootElement.GetProperty("current_condition")[0];
                dto.Temperature = cond.GetProperty("temp_C").GetString() + "°C";
                dto.Description = cond.GetProperty("weatherDesc")[0].GetProperty("value").GetString();
            }
            catch { dto.Description = "Veri alınamadı"; }
            return dto;
        }

        public async Task<GoldDto> GetGoldPricesAsync()
        {
            var dto = new GoldDto();
            try
            {
                var response = await _httpClient.GetStringAsync("https://api.genelpara.com/embed/altin.json");
                using var json = JsonDocument.Parse(response);

                if (json.RootElement.TryGetProperty("GA", out var gaProp) &&
                    gaProp.TryGetProperty("satis", out var gramProp))
                    dto.Gram = gramProp.GetString();

                if (json.RootElement.TryGetProperty("C", out var cProp) &&
                    cProp.TryGetProperty("satis", out var quarterProp))
                    dto.Quarter = quarterProp.GetString();
            }
            catch
            {
                dto.Gram = "0";
                dto.Quarter = "0";
            }
            return dto;
        }
    }
}
