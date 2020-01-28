using System;
using System.Collections.Generic;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace WeatherCollector
{
    class Program
    {
        private static ServiceProvider serviceProvider;

        private static List<string> cities;

        private static ILogger<Program> logger;

        private static IConfiguration configuration;

        static void Main(string[] args)
        {
            ApplicationConfiguration();

            cities = ParseCities(args);

            var weatherCollector = new WeatherCollector(
                configuration,
                serviceProvider.GetService<IWeatherData>(),
                serviceProvider.GetService<ILoggerFactory>().CreateLogger<WeatherCollector>()
            ); 

            weatherCollector.Authorize();

            weatherCollector.CollectWeatherInformation(cities);
        }

        private static string RemoveComma(string word)
        {
            return word.Replace(",", string.Empty);
        }

        private static List<string> ParseCities(string[] args)
        {
            var argsLength = args.Length;

            if (argsLength < 4)
            {
                throw new ArgumentException("Incorrect number of arguments");
            }

            int startCitiesList;

            for (startCitiesList = 0; startCitiesList < argsLength; startCitiesList++)
            {
                if (args[startCitiesList] == "--city")
                {
                    startCitiesList++;
                    break;
                }
            }

            List<string> cities = new List<string>();

            for (var citiesIndex = startCitiesList; citiesIndex < argsLength; citiesIndex++)
            {
                string cityName = RemoveComma(args[citiesIndex]);
                cities.Add(cityName);
            }

            return cities;
        }

        private static void ApplicationConfiguration()
        {
            var builder = new ConfigurationBuilder()
             .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#else
            builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
#endif
            configuration = builder.Build();

            var services = new ServiceCollection()
                .AddLogging();

            services.AddDbContextPool<WeatherCollectorDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("WeatherCollectorDb"));
            });

            services.AddSingleton<IWeatherData, WeatherData>();

            serviceProvider = services.BuildServiceProvider();

            logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogInformation("Application started");
        }
    }
}
