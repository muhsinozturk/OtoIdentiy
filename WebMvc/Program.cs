using Application;
using Infrastructure;
using Infrastructure.Identity;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Katman bağımlılıkları
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Identity şifre Ayarları büyük kücük zorunlu değil vs..
builder.Services.Configure<IdentityOptions>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
});

// Cookie Ayarları → restart sonrası tekrar şifre istesin
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/User/Login";       // giriş yapılmazsa buraya yönlendir
    options.AccessDeniedPath = "/User/Login"; // yetki yoksa buraya yönlendir
    options.Cookie.HttpOnly = true;          // JS erişemesin, güvenlik için
    options.ExpireTimeSpan = TimeSpan.FromSeconds(5); // cookie sadece 5 saniye geçerli
    options.SlidingExpiration = false;       // her istekte süre uzamasın
    options.Cookie.IsEssential = true;       // GDPR için önemli cookie işareti
});

// SecurityStamp → her request’te cookie doğrulansın
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

var app = builder.Build();

// 🔹 Admin Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

    var adminEmail = "admin@site.com";
    var adminPassword = "Admin123!";

    // Rol yoksa oluştur
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
    }

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            Console.WriteLine("✅ Admin kullanıcı ve rol oluşturuldu.");
        }
        else
        {
            Console.WriteLine("❌ Hata: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Uygulama açıldığında otomatik olarak /Admin/Home/Index'e yönlendir
app.MapGet("/", context =>
{
    context.Response.Redirect("/Admin/Home/Index");
    return Task.CompletedTask;
});

app.Run();
