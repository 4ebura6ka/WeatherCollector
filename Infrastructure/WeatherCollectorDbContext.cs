using System;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class WeatherCollectorDbContext : DbContext
    {
        public DbSet<CityWeatherEntity> cityWeathers { get; set; }

        public WeatherCollectorDbContext(DbContextOptions<WeatherCollectorDbContext> options) : base (options)
        {
        }
    }
}
