using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.Infrastructure.Identity
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync(AppRoles.Staff))
                await roleManager.CreateAsync(new IdentityRole(AppRoles.Staff));

            // Credentials come from configuration (user secrets locally, Azure settings in production),
            // never from the repository.
            var email = configuration["StaffAccount:Email"];
            var password = configuration["StaffAccount:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                    throw new InvalidOperationException(
                        "Could not create the staff account: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(user, AppRoles.Staff))
                await userManager.AddToRoleAsync(user, AppRoles.Staff);
        }
    }
}
