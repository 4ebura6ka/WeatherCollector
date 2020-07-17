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

        public WeatherEntity Save (WeatherEntity cityWeather)
        {
            var city = GetCityWeather(cityWeather.City);

            if (city != null)
            {
                city.Weather = cityWeather.Weather;
                city.City = cityWeather.City;
                city.Precipitation = cityWeather.Precipitation;
                city.TemperatureC = cityWeather.TemperatureC;

                return Update(city);
            }

            return Add(cityWeather);
        }

        public WeatherEntity Add (WeatherEntity cityWeather)
        {
            weatherCollectorDbContext.Add(cityWeather);
            return cityWeather;
        }

        public WeatherEntity Update (WeatherEntity cityWeather)
        {
            var entity = weatherCollectorDbContext.cityWeathers.Attach(cityWeather);
            entity.State = EntityState.Modified;

            return cityWeather;
        }

        public WeatherEntity GetCityWeather (string name)
        {
            return weatherCollectorDbContext.cityWeathers.SingleOrDefault(x => x.City == name);
        }

        public int Commit()
        {
            return weatherCollectorDbContext.SaveChanges();
        }

    }
}
