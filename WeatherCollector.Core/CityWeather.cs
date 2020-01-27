using System;
namespace WeatherCollector.Core
{
    public class CityWeather
    {
        public string City { get; set; }

        public double? Temperature { get; set; }

        public int? Precipitation { get; set; }

        public string Weather { get; set; }
    }
}
