using System;
using IO.Swagger.Model;
using IO.Swagger.Api;
using IO.Swagger.Client;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Infrastructure;

namespace WeatherCollector
{
    public class WeatherCollector
    {
        public ApiClient apiClient { get; set; }

        private string authenticationHeader;

        private readonly IConfiguration configuration;
        private readonly IWeatherData weatherData;

        public WeatherCollector(IConfiguration configuration, IWeatherData weatherData)
        {
            this.configuration = configuration;
            this.weatherData = weatherData;

            ApiClient ApiClient = new ApiClient(configuration.GetConnectionString("ApiBasePath"));
        }

        public void Authorize()
        {
            AuthorizationApi authorizationApi = new AuthorizationApi(apiClient);

            AuthorizationRequest authorizationRequest = new AuthorizationRequest();
            authorizationRequest.Username = configuration.GetConnectionString("ApiUser");
            authorizationRequest.Password = configuration.GetConnectionString("ApiPass");

            AuthorizationResponse authorizationResponse = authorizationApi.Authorize(authorizationRequest);
            authenticationHeader = "bearer " + authorizationResponse.Bearer;
        }

        public void CollectWeatherInformation(List<string> cities)
        {
            WeatherApi weatherApi = new WeatherApi(apiClient);

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

            weatherData.Save(city);
            weatherData.Commit();
        }
    }
}
