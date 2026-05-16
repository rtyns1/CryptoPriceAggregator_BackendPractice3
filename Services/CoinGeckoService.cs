
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CryptoPriceAggregator_BackendPractice3;
using CryptoPriceAggregator_BackendPractice3.Models;
using CryptoPriceAggregator_BackendPractice3.Services;
using CryptoPriceAggregator_BackendPractice3.Helpers;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace CryptoPriceAggregator_BackendPractice3.Services
{
    public class CoinGeckoService : ICryptoPriceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        
        public CoinGeckoService(HttpClient httpClient, string apiKey, string baseUrl)
        {
            this._httpClient = httpClient;
            this._apiKey = apiKey;
            this._baseUrl = baseUrl;
        }

        public  async Task <decimal> GetPriceAsync(string symbol)
        {
            // first i need to see the kind of JSON response i get when i calll the API
            // but im only interested in the price yes? so when im concstructing the string URL, i need to consider this part
            try
            {
                // i need to be careful when im constructing URL.
                // what do i need?
                // for CoinCap service, it is ::$"{_baseUrl}/{symbol.ToLower()}?apiKey={_apiKey}"
                // URL construction for Coinggesko is different-- u need to read the docs because every API has a differnnt URL pattern.
                // but fo coingecko, its something like this::https://api.coingecko.com/api/v3/simple/price?vs_currencies=usd&ids=bitcoin
                // now, from this i need to construct the URL, 
                // first, identify the base URL and the key parameters
                //Base URL is : https://api.coingecko.com/api/v3/simple/price
                // key parameters here are vs_currencies and ids, where vs_currencies is the currency we wan which is USD, and ids is the crypto symbol in lowercase so we say symbol.ToLower()
                // start with base url "{_baseUrl}"
                // append the ? character to indicate the start of query parameters-- thisis dtandard in URL construction
                // append the first query parameter, which is vs_currencies= usd
                // append the id parameter wich is "ids=" + symbol.ToLower()
                // append the & to separate paarameters
                // append te new vs_currencies parameter which is "vs_currencies" = usd

                string url = $"{_baseUrl}?ids={symbol.ToLower()}&vs_currencies=usd&x_cg_demo_api_key={_apiKey}";
                string JsonString = await _httpClient.GetStringAsync(url);

                if (string.IsNullOrWhiteSpace(JsonString))
                {
                    throw new Exception("Receinved empty response from the CoinGGecko API.");
                }

                using JsonDocument doc = JsonDocument.Parse(JsonString);
                JsonElement root = doc.RootElement;
                JsonElement data = root.GetProperty(symbol.ToLower());

                if (data.TryGetProperty("usd", out JsonElement priceElement))// does CoinGecko API use data?
                {
                    decimal price = priceElement.GetDecimal();
                    if (decimal.TryParse(price.ToString(), out decimal priceString ))
                    {
                        return price;
                    }
                    else
                    {
                        throw new Exception($"failed to parse price from CoinGecko response: {priceString}");
                    }
                }
                else
                {
                    throw new Exception("Price information is not found in CoinGecko API response.");
                }
            }
            catch (HttpRequestException ex)
            {
                await FileLogger.LogErrorAsync($"CoinGeckoAPIservice HTTP request failed: {ex.Message}");
                throw new Exception($"Failed to call CoinGeckoService: {ex.Message}", ex);
            }

            catch (JsonException ex)
            {
                throw new Exception("Error parsing API response.");
            }

        }

    }
}