using System;
using System.Collections.Generic;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.IO;
using IO.Swagger.Api;
using IO.Swagger.Client;
using Microsoft.Extensions.Logging.Console;
using System.Timers;
using System.Linq;

namespace WeatherCollector
{
    class Program
    {
        private static ServiceProvider serviceProvider;

        private static List<string> cities;

        private static ILogger<Program> logger;

        private static IConfiguration configuration;

        private static WeatherCollector weatherCollector;

        private static InformationDisplay informationDisplay = new InformationDisplay();

        private static Timer timer;

        static void Main(string[] args)
        {
            ApplicationConfiguration();

            ArgumentsParser argumentsParser = new ArgumentsParser();
            cities = argumentsParser.ParseCities(args);
            
            WeatherCollectorSetup();

            RetrieveCitiesWeatherInformation();

            SetTimer();

            while (Console.ReadKey().Key != ConsoleKey.Enter) { }

            timer.Stop();
            timer.Dispose();
        }

        private static void WeatherCollectorSetup()
        {
            weatherCollector = new WeatherCollector(
                configuration,
                serviceProvider.GetService<IWeatherData>(),
                serviceProvider.GetService<ILoggerFactory>().CreateLogger<WeatherCollector>()
            );

            ApiClient apiClient = new ApiClient(configuration.GetConnectionString("ApiBasePath"));
            weatherCollector.ApiClient = apiClient;

            weatherCollector.WeatherApi = new WeatherApi(apiClient);
            weatherCollector.AuthorizationApi = new AuthorizationApi(apiClient);
            weatherCollector.CitiesApi = new CitiesApi(apiClient);

            weatherCollector.Authorize();
        }

        private static void SetTimer()
        {
            timer = new Timer(30000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private static void RetrieveCitiesWeatherInformation()
        {
            var citiesWeatherInformation = weatherCollector.CollectWeatherInformation(cities).ToList();

            informationDisplay.Display(citiesWeatherInformation);

            Console.WriteLine("\nPress <Enter> to exit the process...\n");
        }

        private static void OnTimedEvent (Object source, ElapsedEventArgs e)
        {
            RetrieveCitiesWeatherInformation();
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
                .AddLogging(builder => builder
                    .AddConsole()
                    .AddFilter(level => level >= LogLevel.Warning));

            services.AddDbContextPool<WeatherCollectorDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("WeatherCollectorDb"));
            });

            services.AddSingleton<IWeatherData, WeatherData>();

            serviceProvider = services.BuildServiceProvider();

            logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Application started");
        }
    }
}
