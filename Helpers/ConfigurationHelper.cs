using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.ExceptionServices;

/*
 * Need to download a few packages to work with configuration
 * Microsoft.Extemsions.Configuration
 * Microsoft.Extension.Configuration.Json
 * Microsoft.Extensions.Configuration.Binder
 * using System.Threading
 */
namespace CryptoPriceAggregator_BackendPractice3.Helpers
{
    public class ConfigurationHelper
    {
        // need to understand why we need this class, why its needed, and then how to write it and how to write it well
        // thic class is responsible for reading settings from external sourseces and providing thrm to the rest of the application ni a stronyl types, easy to use way
        /*
         * It Loads config files::-uses ConfigurationBuilder to read appsettings.json (base) and optionally environment specific files like appsettings.Development.json
         * It merges settings frmo multiple sources -- values fromt he environment specific file override the base file.
         * Exposes values -- it provides methods like GetValue<T> or properties so that other classes can access settings without knowingh where they came from.
         * Centralises configuration -- instead of scattering hardcoded strings across your code, you have one place that manages all settings.
         * It is needed and essential for separation of concerns-- business logic/service does not need to know about file paths, JSON parsing, or which environment you are running
         * also, resusab;e
         * 
         * So, now before i write it, i need to atleast have an idea of the methods i should expect it to have and some pseudocode
         * 
         */

        private readonly IConfiguration? _configuration; // this is where the settings will be stored after being loaded from the files
        // this is standard, bcz IConfiguration is the interface provided by Microsoft.Extensions.Configuration, and it allows us to access configuration values in a consistent way, regardless of the source.
        // private and readonly bcz we only want this field to be set once, and only within this class.
        // we need a constructor to load the configuration when an instance of this class is created

        public ConfigurationHelper()
        {
            _configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                   .Build();
            // the section above is called a fluent interface - it is chaining method calls together. Each method call returns the same object, so we can call another method on it immediately.
            // We could also write it in a ismpler way:
            // var builder = new ConfigurationBuilder():
            // builder.SetBasePath(Directory.GetCurrentDirectory()):
            // builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange : true):
            // builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange : true):
            // _configuration = builder.Build();
            // But the fluent interface is more concise and easier to read .
            // the details on how to write this are in the Microsoft Documentation for ConfigurationBuilder
        }

        public T GetValue<T>(string Key)
        {
            return _configuration.GetValue<T>(Key);
        }

        public IConfigurationSection GetSection(String SectionName)
        {
            return _configuration.GetSection(SectionName);
        }

        // All of these are in the Microsoft Documentation for ConfigurationBuilder.
        // 
    }
}

