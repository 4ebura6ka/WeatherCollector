using System;
using IO.Swagger.Model;
using IO.Swagger.Api;
using IO.Swagger.Client;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Infrastructure;

namespace WeatherCollector
{
    public class WeatherCollector
    {
        public ApiClient ApiClient { get; set; }

        private string authenticationHeader;

        private readonly IConfiguration configuration;
        private readonly IWeatherData weatherData;
        private readonly ILogger<WeatherCollector>  logger;

        public WeatherCollector(IConfiguration configuration, IWeatherData weatherData, ILogger<WeatherCollector> logger)
        {
            this.configuration = configuration;
            this.weatherData = weatherData;
            this.logger = logger;

            ApiClient = new ApiClient(configuration.GetConnectionString("ApiBasePath"));
        }

        public void Authorize()
        {
            AuthorizationApi authorizationApi = new AuthorizationApi(ApiClient);

            AuthorizationRequest authorizationRequest = new AuthorizationRequest();
            authorizationRequest.Username = configuration.GetConnectionString("ApiUser");
            authorizationRequest.Password = configuration.GetConnectionString("ApiPass");

            AuthorizationResponse authorizationResponse = authorizationApi.Authorize(authorizationRequest);
            authenticationHeader = "bearer " + authorizationResponse.Bearer;
        }

        public void CollectWeatherInformation(List<string> cities)
        {
            WeatherApi weatherApi = new WeatherApi(ApiClient);

            foreach (var city in cities)
            {
                CityWeather cityWeather = weatherApi.GetWeatherForCity(city, authenticationHeader);
                Save(cityWeather);
            }
        }

        public void Save(CityWeather cityWeather)
        {
            CityWeatherEntity city = new CityWeatherEntity()
            {
                City = cityWeather.City,
                Precipitation = cityWeather.Precipitation,
                Temperature = cityWeather.Temperature,
                Weather = cityWeather.Weather
            };

            logger.LogDebug(cityWeather.ToString());

            weatherData.Save(city);
            weatherData.Commit();
        }
    }
}
