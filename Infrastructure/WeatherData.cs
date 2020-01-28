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
            if (GetWeatherByCity(cityWeather.City).Count() > 0)
            {
                return Update(cityWeather);
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

        public IEnumerable<CityWeatherEntity> GetWeatherByCity (string name)
        {
            return weatherCollectorDbContext.cityWeathers.Where(x => x.City == name);
        }

        public int Commit()
        {
            return weatherCollectorDbContext.SaveChanges();
        }

    }
}
