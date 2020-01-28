using System;
using System.Collections.Generic;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace WeatherCollector
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        private static List<string> cities;

        static void Main(string[] args)
        {
            ApplicationConfiguration();

            cities = ParseCities(args);
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
             .SetBasePath(Directory.GetCurrentDirectory())
             .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            var services = new ServiceCollection()
                .AddLogging();

            services.AddDbContextPool<WeatherCollectorDbContext>((Action<DbContextOptionsBuilder>)(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("WeatherCollectorDb"));
            }));

            services.AddSingleton<IWeatherData, WeatherData>();

            var serviceProvider = services.BuildServiceProvider();
        }
    }
}
