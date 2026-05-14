using System;
using System.Net.Http;
using System.Threading.Tasks;
using CryptoPriceAggregator_BackendPractice3.Helpers;
using CryptoPriceAggregator_BackendPractice3.Services;

namespace CryptoPriceAggregator_BackendPractice3
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load configuration
            var config = new ConfigurationHelper();

            string baseUrl = config.GetValue<string>("CoinCap:BaseUrl");
            string apiKey = config.GetValue<string>("CoinCap:ApiKey") ?? ""; // CoinCap doesn't need key

            // Create HttpClient (singleton)
            var httpClient = new HttpClient();

            // Create service (no circuit breaker, no retry)
            var coinCapService = new CoinCapService(httpClient, apiKey, baseUrl);

            try
            {
                string symbol = config.GetValue<string>("Crypto:Symbol");
                decimal price = await coinCapService.GetPriceAsync(symbol);

                Console.WriteLine($"CoinCap price for {symbol}: ${price}");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

    }

}
