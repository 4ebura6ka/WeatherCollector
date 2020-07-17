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

        public void PrintConsole (List<WeatherEntity> citiesWeather)
        {
            foreach (var cityWeather in citiesWeather)
            {
                Console.WriteLine(cityWeather);
            }
        }
    }
}
