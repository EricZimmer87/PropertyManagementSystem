using Microsoft.AspNetCore.Identity;
using PropertyManagement.Api.Models;

namespace PropertyManagement.Api.Seeding
{
    public class AdminSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILogger<AdminSeeder>>();

            var adminSettings = config.GetSection("AdminSettings");
            var adminUserName = adminSettings["UserName"];
            var adminEmail = adminSettings["Email"]!;
            var adminPassword = adminSettings["Password"]!;
            var adminFirstName = adminSettings["FirstName"]!;
            var adminLastName = adminSettings["LastName"]!;

            // 1. Seed Roles
            foreach (var role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Role '{RoleName}' created.", role);
                }
            }

            // 2. Seed Admin User
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new AppUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    FirstName = adminFirstName,
                    LastName = adminLastName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    // 3. Assign Role
                    await userManager.AddToRoleAsync(user, Roles.Admin);
                    logger.LogInformation("Admin user '{UserName}' created and assigned to {Roles.Admin} role.", adminUserName, Roles.Admin);
                }
                else
                {
                    // Error Handling and Logging
                    foreach (var error in result.Errors)
                    {
                        logger.LogError("Error seeding admin: {Code} - {Description}", error.Code, error.Description);
                    }
                }
            }
            else
            {
                logger.LogInformation("Admin user already exists.");
            }
        }
    }
}
