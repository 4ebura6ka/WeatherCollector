using System.Collections.Generic;

namespace Infrastructure
{
    public interface IWeatherData
    {
        CityWeather Save(CityWeather cityWeather);

        CityWeather Update(CityWeather cityWeather);

        CityWeather Add(CityWeather cityWeather);

        IEnumerable<CityWeather> GetWeatherByCity(string name);

        int Commit();
    }
}
