using System;
using Microsoft.EntityFrameworkCore;
using WeatherCollector.Core;

namespace Infrastructure
{
    public class WeatheCollectorDbContext : DbContext
    {
        public DbSet<CityWeather> cityWeathers { get; set; }

        public WeatheCollectorDbContext()
        {
        }
    }
}
