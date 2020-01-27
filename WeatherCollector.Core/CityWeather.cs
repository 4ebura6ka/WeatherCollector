using System.ComponentModel.DataAnnotations;

namespace WeatherCollector.Core
{
    public class CityWeather
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string City { get; set; }

        public double? Temperature { get; set; }

        public int? Precipitation { get; set; }

        public string Weather { get; set; }
    }
}
