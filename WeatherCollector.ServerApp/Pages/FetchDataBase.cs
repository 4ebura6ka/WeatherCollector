using Infrastructure;
using IO.Swagger.Api;
using IO.Swagger.Client;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherCollector.ConsoleApp;

namespace WeatherCollector.ServerApp
{
    public class FetchDataBase : ComponentBase
    {
        [Inject]
        ConsoleApp.WeatherCollector WeatherCollector { get; set; }

        public List<CityWeatherEntity> CityWeatherList { get; set; } = new List<CityWeatherEntity>();

        private List<string> ParseCities(string cities)
        {
            var parsedCities = new List<string>();

            var whiteSpaceIndex = cities.IndexOf(" ");

            while (whiteSpaceIndex > 0)
            {
                var cityName = cities.Substring(0, whiteSpaceIndex - 1);
                cities = cities.Substring(whiteSpaceIndex + 1, cities.Length - 1);
                whiteSpaceIndex = cities.IndexOf(" ");
                parsedCities.Add(cityName);
            }
            parsedCities.Add(cities);

            return parsedCities;
        }
        protected override async Task OnInitializedAsync()
        {
        }

        protected async Task CollectCityWeatherInParallelMode()
        {

        }

        protected async Task CollectCityWeatherInAsyncMode()
        {
            var parsedCities = ParseCities("Vilnius, Riga, Moscow");
            CityWeatherList = (await WeatherCollector.CollectWeatherInformationAsync(parsedCities)).ToList();
        }

        protected async Task CollectCityWeatherInSequentialMode()
        {

        }
    }
}