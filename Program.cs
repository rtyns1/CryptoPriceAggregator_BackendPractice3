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

            string coincapbaseUrl = config.GetValue<string>("CoinCap:BaseUrl");
            string coingeckobaseUrl = config.GetValue<string>("CoinGecko:BaseUrl");
            string coincapapiKey = config.GetValue<string>("CoinCap:ApiKey") ?? ""; // CoinCap doesn't need key
            string coingeckoapikey = config.GetValue<string>("CoinGecko: ApiKey");
             
            // Create HttpClient (singleton 
            var httpClient = new HttpClient();

            // Create service (no circuit breaker, no retry)
            var coinCapService = new CoinCapService(httpClient, coincapapiKey, coincapbaseUrl);
            var coinGeckoService = new CoinGeckoService(httpClient, coingeckoapikey, coingeckobaseUrl);

            try
            {
                string coincapsymbol = config.GetValue<string>("Crypto:Symbol");
                decimal coincapprice = await coinCapService.GetPriceAsync(coincapsymbol);

                Console.WriteLine($"CoinCap price for {coincapsymbol}: ${coincapprice}");

                string coingeckosymbol = config.GetValue<string>("Crypto:Symbol");
                decimal coingeckoprice = await coinGeckoService.GetPriceAsync(coingeckosymbol);

                Console.WriteLine($"CoinGEcko price for {coingeckosymbol}: ${coingeckoprice}");

            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }

    }

}
