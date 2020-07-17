using System.Collections.Generic;

namespace Infrastructure
{
    public interface IWeatherData
    {
        WeatherEntity Save(WeatherEntity cityWeather);

        WeatherEntity Update(WeatherEntity cityWeather);

        WeatherEntity Add(WeatherEntity cityWeather);

        WeatherEntity GetCityWeather(string name);

        int Commit();
    }
}
