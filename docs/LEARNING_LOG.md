First of all, we need to understand what the problem is about.
# Challenge: Resilient Cryptocurrency Price Aggregator (Long)

## Objective
Build a console app that fetches the current Bitcoin (BTC) price in USD from **two different public APIs** simultaneously, compares the results, and implements both a **manual circuit breaker** (state machine, thread‑safe) and a **Polly refactor**. You must use defensive programming, structured logging, configuration files, and parallel asynchronous calls.

## Core Concepts Tested (from previous projects)
- Async/await, `HttpClient` as singleton.
- Manual circuit breaker (Closed/Open/HalfOpen, `lock`, `Action<string>` logger, `BrokenCircuitException`).
- `Func<Task<T>>` delegate – passing an async lambda to `ExecuteAsync`.
- JSON parsing with `JsonDocument` and `TryGetProperty` (defensive).
- Configuration with `appsettings.json` + `appsettings.Development.json`; `ConfigurationBuilder`, `.Build()`, `Directory.GetCurrentDirectory()`.
- `dotnet add package` for `Microsoft.Extensions.Configuration` and `Polly`.
- `.csproj` setting `<CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>`.
- Two loggers: `FileLogger` (plain text errors) and `JsonLogger` (structured events).
- Graceful error handling (custom exceptions, no leak of internal details).
- Testing circuit breaker with fake async action (fails 3 times, then succeeds).

## APIs (Free, No API Keys)

| API | Endpoint | Price Path |
|-----|----------|------------|
| CoinGecko | `https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd` | `root.GetProperty("bitcoin").GetProperty("usd").GetDouble()` |
| CoinCap | `https://api.coincap.io/v2/assets/bitcoin` | `root.GetProperty("data").GetProperty("priceUsd").GetString()` → convert to `double` |

## Requirements (Phased)

### Phase 1 – Setup & Configuration
1. Create `ResilientCryptoAggregator` console project.
2. Add NuGet: `Microsoft.Extensions.Configuration`, `.Json`, `.Binder`.
3. Create `appsettings.json` (committed) with:
   ```json
   {
     "Crypto": { "Symbol": "bitcoin", "Currency": "usd", "PriceDifferencePercent": 2.0 },
     "CoinGecko": { "BaseUrl": "https://api.coingecko.com/api/v3/simple/price" },
     "CoinCap": { "BaseUrl": "https://api.coincap.io/v2/assets/bitcoin" }
   }

4. Create appsettings.Development.json (ignored) – optional.

5.Add <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory> in .csproj for both JSON files.
6.Implement ConfigurationHelper (load both files, provide GetValue<T>).

### Phase 2 – Loggers & Models
7.Copy FileLogger.cs and JsonLogger.cs (adjust namespaces).

8.Create PriceData model: SourceApi (string), PriceUsd (double), RetrievedAt (DateTime).

### Phase 3  - Manual Circuit Breaker
9.Write ManualCircuitBreaker class:

Enum CircuitState { Closed, Open, HalfOpen }

Fields: _failureThreshold = 3, _openDurationSeconds = 30, _state, _failureCount, _openTime, _lock, _logger

Constructor (Action<string> logger)

ExecuteAsync<T>(Func<Task<T>> action) with full state machine logic (see weather breaker).

Throws BrokenCircuitException when open.

10.Test the breaker in isolation using a fake async lambda (fails 3 times, then succeeds); keep test in Program.cs temporarily.

### Phase 4 - API services with Manual Circuit Breaker

11.Interface ICryptoPriceService with Task<PriceData> GetPriceAsync(string symbol, string currency).

12.Implement CoinGeckoService and CoinCapService. Each:

Constructor takes HttpClient, IConfiguration (or explicit baseUrl), and ManualCircuitBreaker.

Build the URL (CoinGecko uses query params, CoinCap is static).

Execute HTTP call inside _circuitBreaker.ExecuteAsync.

Parse JSON defensively: check null, use TryGetProperty, handle missing fields gracefully.

On success, return PriceData with correct SourceApi.

On BrokenCircuitException, re‑throw meaningful exception.

On HttpRequestException or JsonException, log with FileLogger and re‑throw.

13.n Program.cs:

Load config, create one HttpClient (singleton).

Create two ManualCircuitBreaker instances (separate failure tracking per API).

Instantiate services, call both in parallel (Task.WhenAll), await results.

Compute relative difference: abs(price1 - price2) / average * 100.

If difference > PriceDifferencePercent (from config), log a JSON warning via JsonLogger (include both prices, difference, timestamp).

Display both prices.

### Phase 5 – Defensive & Graceful Error Handling
14. In each service, after receiving jsonString, check IsNullOrWhiteSpace.

15. Use JsonDocument.Parse inside using.

16. Use TryGetProperty for every expected field. If missing, throw a custom PriceDataNotFoundException with source API name.

17. Catch JsonException and wrap with meaningful message; log with FileLogger.

18. Do not let internal exceptions bubble out – preserve encapsulation.

### Phase 6 – Refactor to Polly
19. Install Polly: dotnet add package Polly.

20.Create PollyCircuitBreakerAdapter (implements same ExecuteAsync<T> signature, wraps AsyncCircuitBreakerPolicy).
21. In Program.cs, replace manual breaker creation with:

csharp '''
var breakerPolicy = Policy
    .Handle<HttpRequestException>()
    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
        onBreak: (ex, delay) => Console.WriteLine($"CB open: {ex.Message}"),
        onReset: () => Console.WriteLine("CB closed"),
        onHalfOpen: () => Console.WriteLine("CB half-open"));
var cb = new PollyCircuitBreakerAdapter(breakerPolicy);
22 .Keep the manual breaker code in a Legacy/ folder for reference.
'''

### Phase 7 – Testing & Documentation
23.Run with real APIs; verify prices, comparison, JSON warnings.

24. Force an API error (wrong URL) to see circuit breaker open.

25. Write README.md, LEARNING_LOG.md, LEARNING_SUMMARY.md explaining components, design choices, and lessons.

### Deliverables
-Full solution with all source files.

-Configuration files (.json), .gitignore, .csproj with copy settings.

-Manual circuit breaker, Polly adapter, services, interface, models, loggers, configuration helper.

-Documentation as above.

### HelpersRestrictions
No AI to write code. Use Microsoft docs, your previous code as reference, and your own reasoning.

Ask only conceptual questions.

Now, once i have a clear set like this, i need to device a paln.
Come up with a path to follow for the implementation, how code builds upon each other and dependancy.

** Next time, we talk of dependancies and flow of code as well as separation of responsibilities. **