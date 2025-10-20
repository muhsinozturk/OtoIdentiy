
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebMvc.Models;

namespace WebMvc.Seeds;

public  class PermissionSeed
{
    public static async Task Seed(RoleManager<AppRole> roleManager) 
    {
        var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");
        var hasAdvancedRole = await roleManager.RoleExistsAsync("AdvancedRole");
        var hasAdminRole = await roleManager.RoleExistsAsync("AdminRole");



        if (!hasBasicRole)
        {
            await roleManager.CreateAsync(new AppRole() { Name = "BasicRole" });

            var basicRole =  (await roleManager.FindByNameAsync("BasicRole"))!;

            await AddReadPermission(basicRole, roleManager);
        }

        if (!hasAdvancedRole)
        {
            await roleManager.CreateAsync(new AppRole() { Name = "AdvancedRole" });
            var basicRole = (await roleManager.FindByNameAsync("AdvancedRole"))!;

            await AddReadPermission(basicRole, roleManager);
            await AddUptadeAndCreatePermission(basicRole, roleManager);
        }

        if (!hasAdminRole)
        {
            await roleManager.CreateAsync(new AppRole() { Name = "AdminRole" });

            var basicRole = (await roleManager.FindByNameAsync("AdminRole"))!;

            await AddReadPermission(basicRole, roleManager);
            await AddUptadeAndCreatePermission(basicRole, roleManager);
            await AddDeletePermission(basicRole, roleManager);
        }

    }

    public static async Task AddReadPermission(AppRole role, RoleManager<AppRole> roleManager)
    {
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Stock.Read));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Order.Read));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Catalog.Read));
    }

    public static async Task AddUptadeAndCreatePermission(AppRole role, RoleManager<AppRole> roleManager)
    {
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Stock.Create));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Order.Create));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Catalog.Create));

        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Stock.Update));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Order.Update));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Catalog.Update));
    }

    public static async Task AddDeletePermission(AppRole role, RoleManager<AppRole> roleManager)
    {
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Stock.Delete));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Order.Delete));
        await roleManager.AddClaimAsync(role, new Claim("Permission", Domain.PermissionsRoot.Permissions.Catalog.Delete));
    }
}
