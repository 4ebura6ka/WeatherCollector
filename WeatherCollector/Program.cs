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
using System.Threading.Tasks;
using System.Diagnostics;

namespace WeatherCollector.ConsoleApp
{
    class Program
    {
        private static ServiceProvider serviceProvider;
        
        private static ILogger<Program> logger;

        private static IConfiguration configuration;

        private static List<string> _cities;

        private static WeatherCollector weatherCollector;

        private static WeatherCollectorApi weatherCollectorApi;

        private static InformationDisplay informationDisplay;

        private static Stopwatch stopWatch = new Stopwatch();

        private static Timer _timer;

        static void Main(string[] args)
        {
            ConfigureServices();

            var argumentsParser = new ArgumentsParser();
            var cities = argumentsParser.ParseCities(args);
            
            weatherCollectorApi = WeatherCollectorApiSetup(configuration, 
                serviceProvider.GetService<ILoggerFactory>().CreateLogger<WeatherCollectorApi>()
                );

            _cities = weatherCollectorApi.FilterAvailableCities(cities);
            
            weatherCollector = new WeatherCollector(
                serviceProvider.GetService<IWeatherData>(),
                weatherCollectorApi,
                serviceProvider.GetService<ILoggerFactory>().CreateLogger<WeatherCollector>()
            );

            var citiesWeather = RetrieveCitiesWeatherInformation(_cities).Result;

            informationDisplay = new InformationDisplay();
            informationDisplay.PrintConsole(citiesWeather.ToList());

            SetTimer();

            while (Console.ReadKey().Key != ConsoleKey.Enter) { }

            _timer.Stop();
            _timer.Dispose();
        }

        private static WeatherCollectorApi WeatherCollectorApiSetup(IConfiguration configuration, ILogger<WeatherCollectorApi> logger)
        {
            var weatherCollectorApi = new WeatherCollectorApi(logger);
        
            ApiClient apiClient = new ApiClient(configuration.GetConnectionString("ApiBasePath"));
            weatherCollectorApi.ApiClient = apiClient;
            weatherCollectorApi.WeatherApi = new WeatherApi(apiClient);
            weatherCollectorApi.AuthorizationApi = new AuthorizationApi(apiClient);
            weatherCollectorApi.CitiesApi = new CitiesApi(apiClient);
            weatherCollectorApi.Authorize(configuration.GetConnectionString("ApiUser"), configuration.GetConnectionString("ApiPass"));

            return weatherCollectorApi;
        }

        private static async Task<ICollection<WeatherEntity>> RetrieveCitiesWeatherInformation(List<string> cities)
        {
            Console.WriteLine("Time elapsed:\n"); 

            stopWatch = Stopwatch.StartNew();
            var citiesWeatherAsync = await weatherCollector.RetrieveWeatherAsync(cities);
            stopWatch.Stop();
            Console.WriteLine($"\tfor async request: {stopWatch.ElapsedMilliseconds}ms");

            stopWatch = Stopwatch.StartNew();
            var citiesWeatherParallel =  weatherCollector.RetrieveWeatherParallel(cities).ToList();
            stopWatch.Stop();
            Console.WriteLine($"\tfor parallel request: {stopWatch.ElapsedMilliseconds}ms");

            stopWatch = Stopwatch.StartNew();
            var citiesWeatherSequential = weatherCollector.RetrieveWeatherSequential(cities).ToList();
            stopWatch.Stop();
            Console.WriteLine($"\tfor sequential request: {stopWatch.ElapsedMilliseconds}ms\n");

            return citiesWeatherSequential;
        }

        private static void SetTimer()
        {
            _timer = new Timer(10000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private static async void OnTimedEvent (Object source, ElapsedEventArgs e)
        {
            var citiesWeather = await RetrieveCitiesWeatherInformation(_cities);
            informationDisplay.PrintConsole(citiesWeather.ToList());

            Console.WriteLine($"\nPress any key to exit...\n");
        }

        private static void ConfigureServices()
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
