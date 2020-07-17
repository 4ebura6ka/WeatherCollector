using IO.Swagger.Model;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Infrastructure;
using System.Threading.Tasks;

namespace WeatherCollector.ConsoleApp
{
    public class WeatherCollector
    {
        private readonly IWeatherData _weatherData;
        private readonly ILogger<WeatherCollector> _logger;
        private readonly WeatherCollectorApi _weatherCollectorApi;

        public WeatherCollector(IWeatherData weatherData, WeatherCollectorApi weatherCollectorApi, ILogger<WeatherCollector> logger)
        {
            _weatherData = weatherData;
            _logger = logger;
            _weatherCollectorApi = weatherCollectorApi;
        }

        public ICollection<WeatherEntity> RetrieveWeatherParallel(List<string> cities)
        {
            var receivedWeather = new List<CityWeather>();

            Parallel.ForEach(cities, city =>
            {
                var cityWeather = _weatherCollectorApi.GetCityWeather(city);
                receivedWeather.Add(cityWeather);
            });

            var savedCitiesWeather = SaveWeather(receivedWeather);

            return savedCitiesWeather;
        }

        public async Task<ICollection<WeatherEntity>> RetrieveWeatherAsync (List<string> cities)
        {
            var receivedWeather = new List<CityWeather>();
            var retrieveCityWeatherTasks = new List<Task<CityWeather>>();

            foreach (var city in cities)
            {
                var task = Task.Run(() => _weatherCollectorApi.GetCityWeather(city));
                retrieveCityWeatherTasks.Add(task);
            }

            var results = await Task.WhenAll(retrieveCityWeatherTasks);
            foreach (var result in results)
            {
                receivedWeather.Add(result);
            }

            var savedCitiesWeather = SaveWeather(receivedWeather);

            return savedCitiesWeather;
        }

        public ICollection<WeatherEntity> RetrieveWeatherSequential(List<string> cities)
        {
            var receivedWeather = new List<CityWeather>();

            foreach (var city in cities)
            {
                var cityWeather = _weatherCollectorApi.GetCityWeather(city);
                receivedWeather.Add(cityWeather);
            }

            var savedCitiesWeather = SaveWeather(receivedWeather);

            return savedCitiesWeather;
        }
        private List<WeatherEntity> SaveWeather(List<CityWeather> receivedWeather)
        {
            var savedCitiesWeather = new List<WeatherEntity>();
            foreach (var city in receivedWeather)
            {
                var cityWeatherEntity = SaveCityWeatherData(city);
                savedCitiesWeather.Add(cityWeatherEntity);
            }

            _weatherData.Commit();

            return savedCitiesWeather;
        }

        private WeatherEntity SaveCityWeatherData(CityWeather cityWeather)
        {
            WeatherEntity city = new WeatherEntity()
            {
                City = cityWeather.City,
                Precipitation = cityWeather.Precipitation,
                TemperatureC = cityWeather.Temperature,
                Weather = cityWeather.Weather
            };

            _logger.LogInformation($"Saved data: {cityWeather}");

            //_weatherData.Save(city);

            return city;
        }
    }
}
