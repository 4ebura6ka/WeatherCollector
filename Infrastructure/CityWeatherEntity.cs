using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure
{
    public class CityWeatherEntity
    {
        public int Id { get; set; }

        [Required, StringLength(80)]
        public string City { get; set; }

        public double? Temperature { get; set; }

        public int? Precipitation { get; set; }

        public string Weather { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("  City: ").Append(City).Append("\n");
            sb.Append("  Temperature: ").Append(Temperature).Append("\n");
            sb.Append("  Precipitation: ").Append(Precipitation).Append("\n");
            sb.Append("  Weather: ").Append(Weather).Append("\n");
            return sb.ToString();
        }
    }
}
