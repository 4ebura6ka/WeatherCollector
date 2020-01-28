using System;
using IO.Swagger.Model;
using IO.Swagger.Api;
using IO.Swagger.Client;
using System.Collections.Generic;

namespace WeatherCollector
{
    public class WeatherCollector
    {
        public ApiClient ApiClient { get; set; }

        private string bearer;

        public WeatherCollector()
        {
            ApiClient ApiClient = new ApiClient("http://metasite-weather-api.herokuapp.com");
        }

        public void Authorize()
        {
            AuthorizationApi authorizationApi = new AuthorizationApi(ApiClient);

            AuthorizationRequest authorizationRequest = new AuthorizationRequest();
            authorizationRequest.Username = "meta";
            authorizationRequest.Password = "site";

            AuthorizationResponse authorizationResponse = authorizationApi.Authorize(authorizationRequest);
            bearer = authorizationResponse.Bearer;
        }

        public void CollectWeatherInformation(List<string> cities)
        {
            WeatherApi weatherApi = new WeatherApi(ApiClient);
            CityWeather cityWeather = weatherApi.GetWeatherForCity("Vilnius", "bearer " + bearer);
        }

        public void Save()
        {
         /*   Infrastructure.CityWeather city = new Infrastructure.CityWeather()
            {
                City = cityWeather.City,
                Precipitation = cityWeather.Precipitation,
                Temperature = cityWeather.Temperature,
                Weather = cityWeather.Weather
            };

            serviceProvider.GetService<IWeatherData>().Save(city);
            serviceProvider.GetService<IWeatherData>().Commit();*/
        }
    }
}
