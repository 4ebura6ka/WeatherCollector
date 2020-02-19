using System;
using System.Collections.Generic;

namespace WeatherCollector
{
    public class ArgumentsParser
    {
        public ArgumentsParser()
        {
        }

        private string RemoveComma(string word)
        {
            return word.Replace(",", string.Empty);
        }

        public List<string> ParseCities(string[] args)
        {
            var argsLength = args.Length;

            if (argsLength < 4)
            {
                throw new ArgumentException("Incorrect number of arguments");
            }

            int startCitiesList;

            for (startCitiesList = 0; startCitiesList < argsLength; startCitiesList++)
            {
                if (args[startCitiesList] == "--city")
                {
                    startCitiesList++;
                    break;
                }
            }

            List<string> cities = new List<string>();

            for (var citiesIndex = startCitiesList; citiesIndex < argsLength; citiesIndex++)
            {
                string cityName = RemoveComma(args[citiesIndex]);
                cities.Add(cityName);
            }

            return cities;
        }
    }
}
