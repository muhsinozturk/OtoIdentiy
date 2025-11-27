using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Seed
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            CancellationToken cancellationToken = default)
        {
            // 1) Admin rolü garanti
            var adminRole = await roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == "Admin", cancellationToken);

            if (adminRole == null)
            {
                adminRole = new AppRole { Name = "Admin", NormalizedName = "ADMIN" };

                var roleResult = await roleManager.CreateAsync(adminRole);
                if (!roleResult.Succeeded)
                    throw new Exception("Admin role could not be created: " +
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
            }

            // 2) E-posta ile kullanıcıyı bul
            const string adminEmail = "admin@gmail.com";
            var user = await userManager.FindByEmailAsync(adminEmail);

            // 3) Yoksa oluştur
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "owner",
                    Email = adminEmail,
                   
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, "Admin123*");
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Admin user could not be created: {errors}");
                }
            }

            // 4) Role ekle
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                var addRoleResult = await userManager.AddToRoleAsync(user, "Admin");
                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                    throw new Exception($"Role assignment failed: {errors}");
                }
            }
        }
    }
}
