configuration files, two‑file system, merging, loading

# APPSETTINGS AND CONFIGURATION

in .NET, configuration is typically handled through appsettings.json files. These files allow you to store various settings and values that your application can read at runtime.
We have 2: appsettings.json and appsettings.Development.json.
- appsettins.json is the default configuration file that contains settings for all environments.
- settings means that we can have different values for development, staging, production
- settings are configurable values or parameters that allow software to change ite behavuour, appearance or functionality without altering the underlying source code.
- Configuration means changeing the settings or parameters of a software application to modify its behaviour, appearance or functionality to suit specific needs or preferences.

-settings are values that can change without recompiling code such as API, URLs, database connection strings etc.
- appsettings.json hlds safe defaults, can be commited to git. 
- appsettings.Development.json and sometimes appsettings.Production.json are used to override values in appsettings.json for specific environments, and they should not be commited to git if they have sensitive information.
- sensitive info could be stuff like API keys, database connection strings, passwords, secrets etc etc
- So in a program like ours, there is a simple separation of concerns:::
- models are pure data containers.
- services are responsible for business logic and API interactions, they use loggers to log events and errors, and they use the configuration helper to get settings from the appsettings files.
- helpers are utility classes that provide some common functionality such as logging and configuration loading, they are used by the service classes to perform their tasks, but they are independent of business logic.
- program.cs orchestrates everything

Now, lets start with appsettings.json for our program.

### appsettings.json

Now, this controls the default settnigs like the URLs for the APIs and the log file paths. It should be commited to git, and it should not contain any sensitive information. It can have safe defaults for the API URLs and log file paths.
```json
{
  "CoinGecko": {
	"ApiUrl": "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd"
  },
  "CoinCap": {
	"ApiUrl": "https://api.coincap.io/v2/assets/bitcoin"
  },
  "Logging": {
	"FileLoggerPath": "logs/log.txt",
	"JsonLoggerPath": "logs/events.json"
  }
}
```