using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoPriceAggregator_BackendPractice3;
using CryptoPriceAggregator_BackendPractice3.Models;
using CryptoPriceAggregator_BackendPractice3.Services;
using CryptoPriceAggregator_BackendPractice3.Helpers;
using System.Text.Json;

namespace CryptoPriceAggregator_BackendPractice3.Services
{
    public class CoinCapService : ICryptoPriceService // inherits from the interface, which means we must use the method inside the interface.
    {
        //fields
        //first we are implementing without circuitbreaker and retryhandler, using only httpclient.
        // to see that we can fetch from the API and that the parsing logic works, then we will add those other features.
        // will also allow me to see how th elayers work together, and how to add features to code instead of trying to do everything at once

        private readonly HttpClient _httpClient; // this is he standard way to make Http requests in .NET and it si recommended to use a single instance of HttpClient throughout the application, bcz it manages connections and resources efficiently. 
        private readonly string _apiKey;
        private readonly string _baseUrl;

        // constructor

        public CoinCapService(HttpClient httpClient,string apiKey, string baseUrl)
        {
            this._httpClient = httpClient;
            this._apiKey = apiKey;
            this._baseUrl = baseUrl;
            // would work even if i didnt use 'this' keyword, but its good practice to use the keyword
        }
        // now, what methods?
        // we have to implement the method from the interface, which is GetPriceAsync-- this method should make an API call to coincap, get the price for the given crypto symbol, and return it as a decimal value
        public async Task<decimal> GetPriceAsync(string symbol)
        {
            // we will use the HttpClient to make a GET request to the Coincap API endpoint for the given crypto symbol, and then parse the response to extract the price in USD/
            // the endpoint for getting the price of a crypto in Coicap is something like : https://api.coincap.io/v2/assets{id} where the id is the crypto symbol in lowercase, e.g https://api.coincap/io/v2/assets/bitcoin for bitcoin
            // so we will construct thr URL using the BaseUrl and the symbol and then make the get request, and then parse the response to get the price.
            // the json fomrat willl be something like this;:
            /*
             * {
             *   "data": {
             *     "id": "bitcoin",
             *     "rank": "1",
             *     "symbol": "BTC",
             *     "name": "Bitcoin",
             *     "supply": "17193925.0000000000000000",
             *     "maxSupply": "21000000.0000000000000000",
             *     "marketCapUsd": "119150835874.4699281625807300",
             *     "volumeUsd24Hr": "2927959461.1750323310959460",
             *     "priceUsd": "6931.5058555666618359",
             *     "changePercent24Hr": "-0.8101417214350335",
             *     "vwap24Hr": "7175.0663247679233209"
             *   },
             *   "timestamp": 1533581045388
             * }
             */
            // so we need to deserialise this json response, and extract the proceUSD value from the data object, then return it as a decimal 
            try
            {
                string url = $"{_baseUrl}/{symbol.ToLower()}?apiKey={_apiKey}"; // construct the URL for the API endpoint
                string JsonString = await _httpClient.GetStringAsync(url); // direct HTTP call to get the response as a string, we will then parse this string to extract the price

                if (string.IsNullOrWhiteSpace(JsonString))
                {
                    throw new Exception("Received empty response from the CoinCap API.");

                }
                using JsonDocument doc = JsonDocument.Parse(JsonString);
                JsonElement root = doc.RootElement;
                JsonElement data= root.GetProperty("data");

                if (data.TryGetProperty("priceUsd", out JsonElement priceElement))
                {
                    string priceString = priceElement.GetString();
                    if (decimal.TryParse(priceString, out decimal price))
                    {
                        return price;
                    }
                    else
                    {
                        throw new Exception($"Failed to parse price form CoinCapAPI response: {priceString}");

                    }
                }

                else
                {
                    throw new Exception("Price information not found in CoinCap API respose.");
                }

                
                
            }
            catch (HttpRequestException ex)
            {
                await FileLogger.LogErrorAsync($"CoipCapAPIservice HTTP request failed: {ex.Message}");
                throw new Exception($"Failed to call CoinCapService: {ex.Message}", ex);
            }

            catch (JsonException ex)
            {
                throw new Exception("Error parsing API response.");
            }


        }
        
         

        
    }
}


