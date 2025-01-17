using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyShop.Data.Models;

namespace MyShop.Data
{
    public class MyShopDbContext : IdentityDbContext<User>
    {
        public MyShopDbContext(DbContextOptions<MyShopDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}


