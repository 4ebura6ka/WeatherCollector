using System.Collections.Generic;

namespace Infrastructure
{
    public interface IWeatherData
    {
        CityWeatherEntity Save(CityWeatherEntity cityWeather);

        CityWeatherEntity Update(CityWeatherEntity cityWeather);

        CityWeatherEntity Add(CityWeatherEntity cityWeather);

        IEnumerable<CityWeatherEntity> GetWeatherByCity(string name);

        int Commit();
    }
}
