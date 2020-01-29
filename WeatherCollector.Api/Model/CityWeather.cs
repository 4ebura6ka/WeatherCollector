using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace IO.Swagger.Model {

  /// <summary>
  /// 
  /// </summary>
  [DataContract]
  public class CityWeather {
    /// <summary>
    /// Gets or Sets City
    /// </summary>
    [DataMember(Name="city", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "city")]
    public string City { get; set; }

    /// <summary>
    /// Gets or Sets Temperature
    /// </summary>
    [DataMember(Name="temperature", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "temperature")]
    public double? Temperature { get; set; }

    /// <summary>
    /// Gets or Sets Precipitation
    /// </summary>
    [DataMember(Name="precipitation", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "precipitation")]
    public int? Precipitation { get; set; }

    /// <summary>
    /// Gets or Sets Weather
    /// </summary>
    [DataMember(Name="weather", EmitDefaultValue=false)]
    [JsonProperty(PropertyName = "weather")]
    public string Weather { get; set; }


    /// <summary>
    /// Get the string presentation of the object
    /// </summary>
    /// <returns>String presentation of the object</returns>
    public override string ToString()  {
      var sb = new StringBuilder();
      sb.Append("class CityWeather {\n");
      sb.Append("  City: ").Append(City).Append("\n");
      sb.Append("  Temperature: ").Append(Temperature).Append("\n");
      sb.Append("  Precipitation: ").Append(Precipitation).Append("\n");
      sb.Append("  Weather: ").Append(Weather).Append("\n");
      sb.Append("}\n");
      return sb.ToString();
    }

    /// <summary>
    /// Get the JSON string presentation of the object
    /// </summary>
    /// <returns>JSON string presentation of the object</returns>
    public string ToJson() {
      return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

}
}
