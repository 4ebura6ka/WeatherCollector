using IO.Swagger.Api;
using IO.Swagger.Client;
using IO.Swagger.Model;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace WeatherCollector.ConsoleApp
{
    public class WeatherCollectorApi
    {
        public ApiClient ApiClient { get; set; }

        public WeatherApi WeatherApi { get; set; }

        public AuthorizationApi AuthorizationApi { get; set; }

        public CitiesApi CitiesApi { get; set; }

        private string authorizationHeader;
        private readonly ILogger<WeatherCollectorApi> _logger;

        public WeatherCollectorApi(ILogger<WeatherCollectorApi> logger)
        {
            _logger = logger;
        }
        public void Authorize(string username, string password)
        {
            AuthorizationRequest authorizationRequest = new AuthorizationRequest();
            authorizationRequest.Username = username;
            authorizationRequest.Password = password;

            AuthorizationResponse authorizationResponse = AuthorizationApi.Authorize(authorizationRequest);
            authorizationHeader = "bearer " + authorizationResponse.Bearer;
        }

        public CityWeather GetCityWeather(string city)
        {
            return WeatherApi.GetCityWeather(city, authorizationHeader);
        }
        public List<string> FilterAvailableCities(List<string> cities)
        {
            var availableCities = CitiesApi.GetCities(authorizationHeader);
            var filteredCities = new List<string>();

            foreach (var city in cities)
            {
                if (availableCities.Contains(city))
                {
                    filteredCities.Add(city);
                }
                else
                {
                    _logger.LogWarning($"{city} information was not retrieved, no forecast for mentioned city.");
                }
            }

            return filteredCities;
        }
    }
}
