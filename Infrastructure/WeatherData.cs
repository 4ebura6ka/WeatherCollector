using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class WeatherData : IWeatherData
    {
        private readonly WeatherCollectorDbContext weatherCollectorDbContext;

        public WeatherData(WeatherCollectorDbContext dbContext) {
            weatherCollectorDbContext = dbContext;
        }

        public CityWeatherEntity Save (CityWeatherEntity cityWeather)
        {
            var city = GetCityWeather(cityWeather.City);

            if (city != null)
            {
                city.Weather = cityWeather.Weather;
                city.City = cityWeather.City;
                city.Precipitation = cityWeather.Precipitation;
                city.Temperature = cityWeather.Temperature;

                return Update(city);
            }

            return Add(cityWeather);
        }

        public CityWeatherEntity Add (CityWeatherEntity cityWeather)
        {
            weatherCollectorDbContext.Add(cityWeather);
            return cityWeather;
        }

        public CityWeatherEntity Update (CityWeatherEntity cityWeather)
        {
            var entity = weatherCollectorDbContext.cityWeathers.Attach(cityWeather);
            entity.State = EntityState.Modified;

            return cityWeather;
        }

        public CityWeatherEntity GetCityWeather (string name)
        {
            return weatherCollectorDbContext.cityWeathers.SingleOrDefault(x => x.City == name);
        }

        public int Commit()
        {
            return weatherCollectorDbContext.SaveChanges();
        }

    }
}
