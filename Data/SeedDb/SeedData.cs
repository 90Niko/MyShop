using Microsoft.AspNetCore.Identity;
using MyShop.Data.Models;

namespace MyShop.Data.SeedDb
{
    public static class SeedData
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider, MyShopDbContext context)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

            // Ensure the Admin role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create an admin user
            var adminUser = new User
            {
                UserName = "admin@myshop.com",
                Email = "admin@myshop.com",
                FirstName = "Admin",
                LastName = "Niko",
                EmailConfirmed = true
            };

            var adminPassword = "admin123!";
            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
