using IO.Swagger.Api;
using IO.Swagger.Client;
using NUnit.Framework;
using System;

namespace WeatherCollector.Tests
{
    public class WeatherApiTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetCityWeatherExceptionTest()
        {
            WeatherApi weatherApi = new WeatherApi("http://metasite-weather-api.herokuapp.com");
            ApiException apiException = new ApiException();

            Exception exception = null;

            try
            {
                weatherApi.GetCityWeather(null, null);
            }
            catch (ApiException ex)
            {
                exception = ex;
            }

            Assert.IsNotNull(exception);
        }
    }
}