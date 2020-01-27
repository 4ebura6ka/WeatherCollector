using System;
using Microsoft.EntityFrameworkCore;
using WeatherCollector.Core;

namespace Infrastructure
{
    public class WeatherCollectorDbContext : DbContext
    {
        public DbSet<CityWeather> cityWeathers { get; set; }

        public WeatherCollectorDbContext(DbContextOptions<WeatherCollectorDbContext> options) : base (options)
        {
        }
    }
}
