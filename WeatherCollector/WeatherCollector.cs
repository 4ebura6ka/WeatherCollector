using System;
using IO.Swagger.Model;
using IO.Swagger.Api;
using IO.Swagger.Client;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherCollector
{
    public class WeatherCollector
    {
        public ApiClient ApiClient { get; set; }

        public WeatherApi WeatherApi { get; set; }

        public AuthorizationApi AuthorizationApi { get; set; }

        public CitiesApi CitiesApi { get; set; }

        private string authorizationHeader;

        private readonly IConfiguration configuration;
        private readonly IWeatherData weatherData;
        private readonly ILogger<WeatherCollector> logger;

        public WeatherCollector(IConfiguration configuration, IWeatherData weatherData, ILogger<WeatherCollector> logger)
        {
            this.configuration = configuration;
            this.weatherData = weatherData;
            this.logger = logger;
        }

        public void Authorize()
        {
            AuthorizationRequest authorizationRequest = new AuthorizationRequest();
            authorizationRequest.Username = configuration.GetConnectionString("ApiUser");
            authorizationRequest.Password = configuration.GetConnectionString("ApiPass");

            AuthorizationResponse authorizationResponse = AuthorizationApi.Authorize(authorizationRequest);
            authorizationHeader = "bearer " + authorizationResponse.Bearer;
        }


        public ICollection<CityWeatherEntity> CollectWeatherInformationParallel(List<string> cities)
        {
            var availableCities = CitiesApi.GetCities(authorizationHeader);

            List<CityWeather> receivedWeatherInformation = new List<CityWeather>();

            Parallel.ForEach(cities, city =>
            {
               if (availableCities.Contains(city))
               {
                    var cityWeather = WeatherApi.GetCityWeather(city, authorizationHeader);
                    receivedWeatherInformation.Add(cityWeather);
               }
               else
                {
                    logger.LogWarning($"{city} information was not retrieved, no forecast for mentioned city.");
                }
            });

            List<CityWeatherEntity> savedCitiesWeatherInformation = new List<CityWeatherEntity>();
            foreach (var city in receivedWeatherInformation)
            {
                var cityWeatherEntity = SaveCityWeatherData(city);
                savedCitiesWeatherInformation.Add(cityWeatherEntity);
            }


            weatherData.Commit();

            return savedCitiesWeatherInformation;
        }

        public async Task<ICollection<CityWeatherEntity>> CollectWeatherInformationAsync (List<string> cities)
        {
            var availableCities = CitiesApi.GetCities(authorizationHeader);

            var receivedWeatherInformation = new List<CityWeather>();
            var retrieveCityWeatherTasks = new List<Task<CityWeather>>();

            foreach (var city in cities)
            {
                if (availableCities.Contains(city))
                {
                    var task = Task.Run(() => WeatherApi.GetCityWeather(city, authorizationHeader));
                    retrieveCityWeatherTasks.Add(task);
                }
                else
                {
                    logger.LogWarning($"{city} information was not retrieved, no forecast for mentioned city.");
                }
            }

            var results = await Task.WhenAll(retrieveCityWeatherTasks);

            foreach (var result in results)
            {
                receivedWeatherInformation.Add(result);
            }

            List<CityWeatherEntity> savedCitiesWeatherInformation = new List<CityWeatherEntity>();
            foreach (var city in receivedWeatherInformation)
            {
                var cityWeatherEntity = SaveCityWeatherData(city);
                savedCitiesWeatherInformation.Add(cityWeatherEntity);
            }

            weatherData.Commit();

            return savedCitiesWeatherInformation;
        }

        public ICollection<CityWeatherEntity> CollectWeatherInformationSequential(List<string> cities)
        {
            var availableCities = CitiesApi.GetCities(authorizationHeader);

            List<CityWeather> receivedWeatherInformation = new List<CityWeather>();

            List<Task<CityWeather>> retrieveCityWeatherTasks = new List<Task<CityWeather>>();

            foreach (var city in cities)
            {
                if (availableCities.Contains(city))
                {
                    var cityWeather = WeatherApi.GetCityWeather(city, authorizationHeader);
                    receivedWeatherInformation.Add(cityWeather);
                }
                else
                {
                    logger.LogWarning($"{city} information was not retrieved, no forecast for mentioned city.");
                }
            }

            List<CityWeatherEntity> savedCitiesWeatherInformation = new List<CityWeatherEntity>();
            foreach (var city in receivedWeatherInformation)
            {
                var cityWeatherEntity = SaveCityWeatherData(city);
                savedCitiesWeatherInformation.Add(cityWeatherEntity);
            }

            weatherData.Commit();

            return savedCitiesWeatherInformation;
        }


        private CityWeatherEntity SaveCityWeatherData(CityWeather cityWeather)
        {
            CityWeatherEntity city = new CityWeatherEntity()
            {
                City = cityWeather.City,
                Precipitation = cityWeather.Precipitation,
                Temperature = cityWeather.Temperature,
                Weather = cityWeather.Weather
            };

            logger.LogInformation(cityWeather.ToString());

            //weatherData.Save(city);

            return city;
        }
    }
}
