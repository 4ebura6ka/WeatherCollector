using Infrastructure;
using IO.Swagger.Api;
using IO.Swagger.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WeatherCollector.ConsoleApp;
using WeatherCollector.ServerApp.Data;

namespace WeatherCollector.ServerApp
{
    public class FetchDataBase : ComponentBase
    {
        [Inject]
        ConsoleApp.WeatherCollector WeatherCollector { get; set; }

        [Inject]
        IConfiguration Configuration { get; set; }

        [Inject]
        WeatherCollectorApi WeatherCollectorApi { get; set; }

        private Stopwatch stopWatch;

        public string ExecutionTime { get; set; }

        public List<WeatherEntity> CityWeatherList { get; set; }

        public ForecastModel ForecastModel { get; set; } = new ForecastModel();
        private List<string> ParseCities(string cities)
        {
            var parsedCities = new List<string>();
            var whiteSpaceIndex = cities.IndexOf(" ");

            while (whiteSpaceIndex > 0)
            {
                var cityName = cities.Substring(0, whiteSpaceIndex);
                parsedCities.Add(cityName);
                cities = cities.Substring(whiteSpaceIndex + 1);
                whiteSpaceIndex = cities.IndexOf(" ");
            }
            parsedCities.Add(cities);

            return parsedCities;
        }
        protected override async Task OnInitializedAsync()
        {
            SetWeatherCollectorApi();
        }

        protected void RetrieveCityWeatherParallel()
        {
            var parsedCities = ParseCities(ForecastModel.Cities);

            var _cities = WeatherCollectorApi.FilterAvailableCities(parsedCities);
            stopWatch = Stopwatch.StartNew();
            CityWeatherList = WeatherCollector.RetrieveWeatherParallel(_cities).ToList();
            stopWatch.Stop();
            ExecutionTime = $"parallel - {stopWatch.ElapsedMilliseconds}ms";
        }

        protected async Task RetrieveCityWeatherAsync()
        {
            var parsedCities = ParseCities(ForecastModel.Cities);
            
            var _cities = WeatherCollectorApi.FilterAvailableCities(parsedCities);
            stopWatch = Stopwatch.StartNew();
            CityWeatherList = (await WeatherCollector.RetrieveWeatherAsync(_cities)).ToList();
            stopWatch.Stop();
            ExecutionTime = $"async - {stopWatch.ElapsedMilliseconds}ms";
        }

        protected void RetrieveCityWeatherSequential()
        {
            var parsedCities = ParseCities(ForecastModel.Cities);

            var _cities = WeatherCollectorApi.FilterAvailableCities(parsedCities);
            stopWatch = Stopwatch.StartNew();
            CityWeatherList = WeatherCollector.RetrieveWeatherSequential(_cities).ToList();
            stopWatch.Stop();
            ExecutionTime = $"sequential - {stopWatch.ElapsedMilliseconds}ms";
        }

        private void SetWeatherCollectorApi()
        {
            ApiClient apiClient = new ApiClient(Configuration.GetConnectionString("ApiBasePath"));
            WeatherCollectorApi.ApiClient = apiClient;
            WeatherCollectorApi.WeatherApi = new WeatherApi(apiClient);
            WeatherCollectorApi.AuthorizationApi = new AuthorizationApi(apiClient);
            WeatherCollectorApi.CitiesApi = new CitiesApi(apiClient);
            WeatherCollectorApi.Authorize(Configuration.GetConnectionString("ApiUser"), Configuration.GetConnectionString("ApiPass"));
        }
    }
}