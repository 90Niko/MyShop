﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyShop.Data.Models;
using MyShop.Data.SeedDb;

namespace MyShop.Data
{
    public class MyShopDbContext : IdentityDbContext<User>
    {
        public MyShopDbContext(DbContextOptions<MyShopDbContext> options)
            : base(options)
        {

        }
      
        public async Task EnsureSeededAsync(IServiceProvider serviceProvider)
        {
            await SeedData.SeedAsync(serviceProvider, this);
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

    }
}


