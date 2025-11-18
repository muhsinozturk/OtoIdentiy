using Infrastructure.Identity;
using System.Threading;

namespace Infrastructure.Data.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
        {
            var defaultRoles = new[]
            {
                new AppRole { Name = "Admin", NormalizedName = "ADMIN" }
            };

            foreach (var role in defaultRoles)
            {
                // Role yoksa ekle
                if (!context.Roles.Any(r => r.Name == role.Name))
                {
                    context.Roles.Add(role);
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
