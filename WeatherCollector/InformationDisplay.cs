using System;
using System.Collections.Generic;
using Infrastructure;

namespace WeatherCollector.ConsoleApp
{
    public class InformationDisplay
    {
        public InformationDisplay()
        {
        }

        public void Display (List<CityWeatherEntity> citiesWeatherInformation)
        {
            foreach (var cityWeatherInformation in citiesWeatherInformation)
            {
                Console.WriteLine(cityWeatherInformation);
            }
        }
    }
}
