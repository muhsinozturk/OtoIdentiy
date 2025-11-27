using Application;
using Domain.OptionsModels;
using Domain.PermissionsRoot;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Data.Seed;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using WebMvc.ClaimProvider;
using WebMvc.Extenisons;
using WebMvc.Models;



var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();
// HttpClient
builder.Services.AddHttpClient();
// Katman bağımlılıkları
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
// Identity
builder.Services.Configure<DataProtectionTokenProviderOptions>(options => // Token ömrünü 2 saate çıkarttık
{
    options.TokenLifespan = TimeSpan.FromHours(2); // Token ömrü 2 saat
});

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));// Email ayarlarını configuration dosyasından alıp EmailSettings sınıfına bağla

builder.Services.Configure<SecurityStampValidatorOptions>(options => // Güvenlik damgası doğrulama ayarları
{
    options.ValidationInterval = TimeSpan.FromMinutes(30); // Güvenlik damgası doğrulama aralığı 30 dakika
});

builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Directory.GetCurrentDirectory()));// wwwroot klasörüne erişim için gerekli servis kaydı


builder.Services.AddIdentityWithExt();// Identity servislerini ekle  EXTENİSON klasörüne taşıdık program cs cok kalabalık olduğu için

builder.Services.AddScoped<IClaimsTransformation, UserClaimProvider>();// Claim provider servisinin DI kaydı


builder.Services.AddHttpContextAccessor();// HttpContext erişimi için servis kaydı

builder.Services.AddAuthorization(options =>
{
    /*
    options.AddPolicy("AnkaraPolicy", policy =>
    {
        policy.RequireClaim("city", "Ankara");
    });

    options.AddPolicy("ExchangePolicy", policy =>
    {
        policy.AddRequirements(new ExchangeExpireRequirement());
    });

    options.AddPolicy("ViolencePolicy", policy =>
    {
        policy.AddRequirements(new ViolenceRequirement() { ThresholdAge = 18 });
    });

    // Permission Policies

    options.AddPolicy("OrderPermissionReadAndDelete", policy =>
    {
        policy.RequireClaim("permission", Permissions.Order.Read);
        policy.RequireClaim("permission", Permissions.Order.Delete);
        policy.RequireClaim("permission", Permissions.Stock.Delete);
    });

    options.AddPolicy("Permissions.Order.Read", policy =>
    {
        policy.RequireClaim("permission", Permissions.Order.Read);
    });

    options.AddPolicy("Permissions.Order.Delete", policy =>
    {
        policy.RequireClaim("permission", Permissions.Order.Delete);
    });

    options.AddPolicy("Permissions.Stock.Delete", policy =>
    {
        policy.RequireClaim("permission", Permissions.Stock.Delete);
    });
    */
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/SignIn";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.Cookie.Name = "AspNetCoreIdentityAppCookie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
});

var app = builder.Build();
bool isSeedOnly = args.Contains("--seed");
if (isSeedOnly)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    using var tx = await db.Database.BeginTransactionAsync();

    await tx.CommitAsync();

    // Sıra ile seed işlemleri

    await RoleSeeder.SeedAsync(db);
    await UserSeeder.SeedAsync(userManager, roleManager);

    return;
}


//using (var scope = app.Services.CreateScope()) //
//{
  //  var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    //await PermissionSeed.Seed(roleManager);

//}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication(); // Kimlik doğrulama middleware'i

app.UseAuthorization(); // Yetkilendirme middleware'i

app.MapStaticAssets();


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=SignIn}/{id?}")
    .WithStaticAssets();


app.Run();
