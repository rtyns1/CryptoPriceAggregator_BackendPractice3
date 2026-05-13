using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace CryptoPriceAggregator_BackendPractice3.Helpers
{
    public static class JsonLogger
    {
        private static readonly string JsonLogFilePath = "CryptoPriceAggregator.log";

        public static async Task LogAsync(object data)
        {
            try
            {
                string jsonline = JsonSerializer.Serialize(data);
                await File.AppendAllTextAsync(JsonLogFilePath, jsonline + Environment.NewLine);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"JSON.Logging failed : {ex.Message}");

            }

        }
    }
}

