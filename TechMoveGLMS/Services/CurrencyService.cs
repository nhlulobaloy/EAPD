using System.Text.Json;

namespace TechMoveGLMS.Services
{
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> GetUsdToZarRate()
        {
            try
            {
                string url = "https://api.exchangerate-api.com/v4/latest/USD";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<ExchangeRateResponse>(json);

                if (data?.Rates != null && data.Rates.ContainsKey("ZAR"))
                {
                    return data.Rates["ZAR"];
                }
                return 19.50m;
            }
            catch
            {
                return 19.50m;
            }
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal rate)
        {
            return usdAmount * rate;
        }
    }

    public class ExchangeRateResponse
    {
        public Dictionary<string, decimal> Rates { get; set; } = new Dictionary<string, decimal>();
    }
}