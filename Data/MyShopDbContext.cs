using Microsoft.EntityFrameworkCore;
using MyShop.Data.Models;

namespace MyShop.Data
{
    public class MyShopDbContext : DbContext
    {
        public MyShopDbContext(DbContextOptions<MyShopDbContext> options)
            : base(options)
        {
        }
        public DbSet<WeatherForecast> WeatherForecasts { get; set; } = null!;
        public DbSet<Product> Products { get; set; }= null!;
        public DbSet<Category> Categories { get; set; } = null!;
    }
}
