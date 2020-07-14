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
using WeatherCollector;

namespace WeatherCollector.ServerApp
{
    public class FetchDataBase : ComponentBase
    {
        protected WeatherForecast[] forecasts;

        WeatherCollector weatherCollector;

        [Inject]
        public IConfiguration configuration { get; set; }

        [Inject]
        private IWeatherData weatherData { get; set; }

        [Inject]
        private ILogger<WeatherCollector> logger { get; set; }

        public string Cities { get; set; }
        protected override async Task OnInitializedAsync()
        {
            weatherCollector = new WeatherCollector(configuration, weatherData, logger);

            ApiClient apiClient = new ApiClient(configuration.GetConnectionString("ApiBasePath"));
            weatherCollector.ApiClient = apiClient;

            weatherCollector.WeatherApi = new WeatherApi(apiClient);
            weatherCollector.AuthorizationApi = new AuthorizationApi(apiClient);
            weatherCollector.CitiesApi = new CitiesApi(apiClient);

            weatherCollector.Authorize();
        }

        protected async EventCallback CollectCityWeatherInParallelMode()
        {

        }

        protected async EventCallback CollectCityWeatherInAsyncMode()
        { 
        }

        protected async EventCallBack CollectCityWeatherInSequentialMode()
        {

        }
    }

    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public string Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
