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

        private static Stopwatch stopWatch = new Stopwatch();

        private static Timer _timer;

        static void Main(string[] args)
        {
            ApplicationConfiguration();

            ArgumentsParser argumentsParser = new ArgumentsParser();
            cities = argumentsParser.ParseCities(args);
            
            WeatherCollectorSetup();

            RetrieveCitiesWeatherInformation();

            SetTimer();

            while (Console.ReadKey().Key != ConsoleKey.Enter) { }

            _timer.Stop();
            _timer.Dispose();
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
            _timer = new Timer(10000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private static async Task RetrieveCitiesWeatherInformation()
        {
            Console.WriteLine("Time elapsed:\n"); 
            stopWatch = Stopwatch.StartNew();

            var citiesWeatherInformationAsync = await weatherCollector.CollectWeatherInformationAsync(cities);
            stopWatch.Stop();
            Console.WriteLine($"\tfor async request: {stopWatch.ElapsedMilliseconds}ms");

            stopWatch = Stopwatch.StartNew();

            var citiesWeatherInformationParallel =  weatherCollector.CollectWeatherInformationParallel(cities).ToList();
            stopWatch.Stop();
            Console.WriteLine($"\tfor parallel request: {stopWatch.ElapsedMilliseconds}ms");

            stopWatch = Stopwatch.StartNew();

            var citiesWeatherInformationSequential = weatherCollector.CollectWeatherInformationSequential(cities).ToList();
            stopWatch.Stop();
            Console.WriteLine($"\tfor sequential request: {stopWatch.ElapsedMilliseconds}ms\n");

            // informationDisplay.Display(citiesWeatherInformation.ToList());

            Console.WriteLine($"\nPress any key to exit...\n");
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
