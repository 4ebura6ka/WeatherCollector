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

        public CityWeather Save (CityWeather cityWeather)
        {
            if (GetWeatherByCity(cityWeather.City).Count() > 0)
            {
                return Update(cityWeather);
            }

            return Add(cityWeather);
        }

        public CityWeather Add (CityWeather cityWeather)
        {
            weatherCollectorDbContext.Add(cityWeather);
            return cityWeather;
        }

        public CityWeather Update (CityWeather cityWeather)
        {
            var entity = weatherCollectorDbContext.cityWeathers.Attach(cityWeather);
            entity.State = EntityState.Modified;
            return cityWeather;
        }

        public IEnumerable<CityWeather> GetWeatherByCity (string name)
        {
            return weatherCollectorDbContext.cityWeathers.Where(x => x.City == name);
        }

        public int Commit()
        {
            return weatherCollectorDbContext.SaveChanges();
        }

    }
}
