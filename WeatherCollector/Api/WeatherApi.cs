using System;
using System.Collections.Generic;
using RestSharp;
using IO.Swagger.Client;
using IO.Swagger.Model;

namespace IO.Swagger.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public interface IWeatherApi
    {
        /// <summary>
        /// Returns weather data for specific city. 
        /// </summary>
        /// <param name="city">City from &#x60;Cities&#x60; endpoint.</param>
        /// <param name="authorization">Token received from &#x60;authorization&#x60; endpoint. Provide it as \&quot;&#x60;bearer {token}&#x60;\&quot;.</param>
        /// <returns>CityWeather</returns>
        CityWeather GetCityWeather(string city, string authorization);
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
    public class WeatherApi : IWeatherApi
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherApi"/> class.
        /// </summary>
        /// <param name="apiClient"> an instance of ApiClient (optional)</param>
        /// <returns></returns>
        public WeatherApi(ApiClient apiClient = null)
        {
            if (apiClient == null) // use the default one in Configuration
                this.ApiClient = Configuration.DefaultApiClient;
            else
                this.ApiClient = apiClient;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeatherApi"/> class.
        /// </summary>
        /// <returns></returns>
        public WeatherApi(String basePath)
        {
            this.ApiClient = new ApiClient(basePath);
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <param name="basePath">The base path</param>
        /// <value>The base path</value>
        public void SetBasePath(String basePath)
        {
            this.ApiClient.BasePath = basePath;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <param name="basePath">The base path</param>
        /// <value>The base path</value>
        public String GetBasePath(String basePath)
        {
            return this.ApiClient.BasePath;
        }

        /// <summary>
        /// Gets or sets the API client.
        /// </summary>
        /// <value>An instance of the ApiClient</value>
        public ApiClient ApiClient { get; set; }

        /// <summary>
        /// Returns weather data for specific city. 
        /// </summary>
        /// <param name="city">City from &#x60;Cities&#x60; endpoint.</param> 
        /// <param name="authorization">Token received from &#x60;authorization&#x60; endpoint. Provide it as \&quot;&#x60;bearer {token}&#x60;\&quot;.</param> 
        /// <returns>CityWeather</returns>            
        public CityWeather GetCityWeather(string city, string authorization)
        {

            // verify the required parameter 'city' is set
            if (city == null) throw new ApiException(400, "Missing required parameter 'city' when calling GetWeatherForCity");

            // verify the required parameter 'authorization' is set
            if (authorization == null) throw new ApiException(400, "Missing required parameter 'authorization' when calling GetWeatherForCity");


            var path = "/api/Weather/{city}";
            path = path.Replace("{format}", "json");
            path = path.Replace("{" + "city" + "}", ApiClient.ParameterToString(city));

            var queryParams = new Dictionary<String, String>();
            var headerParams = new Dictionary<String, String>();
            var formParams = new Dictionary<String, String>();
            var fileParams = new Dictionary<String, FileParameter>();
            String postBody = null;

            if (authorization != null) headerParams.Add("Authorization", ApiClient.ParameterToString(authorization)); // header parameter

            // authentication setting, if any
            String[] authSettings = new String[] { };

            // make the HTTP request
            IRestResponse response = (IRestResponse)ApiClient.CallApi(path, Method.GET, queryParams, postBody, headerParams, formParams, fileParams, authSettings);

            if (((int)response.StatusCode) >= 400)
                throw new ApiException((int)response.StatusCode, "Error calling GetWeatherForCity: " + response.Content, response.Content);
            else if (((int)response.StatusCode) == 0)
                throw new ApiException((int)response.StatusCode, "Error calling GetWeatherForCity: " + response.ErrorMessage, response.ErrorMessage);

            return (CityWeather)ApiClient.Deserialize(response.Content, typeof(CityWeather), response.Headers);
        }

    }
}
