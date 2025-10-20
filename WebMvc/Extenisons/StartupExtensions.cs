
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using WebMvc.CustomValidations;
using WebMvc.Localizations;
using WebMvc.Models;

namespace WebMvc.Extenisons
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {


            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true;// Email benzersiz olmalı
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789*.-._@+";// Kullanıcı adı için izin verilen karakterler
                options.Password.RequiredLength = 6;// Parola minimum uzunluğu
                options.Password.RequireNonAlphanumeric = false;// Parolada en az bir alfanümerik karakter zorunluluğu
                options.Password.RequireLowercase = false;// Parolada en az bir küçük harf zorunluluğu
                options.Password.RequireUppercase = false;// Parolada en az bir büyük harf zorunluluğu
                options.Password.RequireDigit = false;// Parolada en az bir rakam zorunluluğu

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);// Kilitlenme süresi 3 dakika

                options.Lockout.MaxFailedAccessAttempts = 3; // 3 başarısız giriş denemesinden sonra kullanıcıyı kilitle
            })
             .AddPasswordValidator<PasswordValidator>()
             .AddUserValidator<UserValidator>() // ✅ Burada doğru şekilde UserValidator ekleniyor
             .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
             .AddDefaultTokenProviders()// Parola sıfırlama gibi işlemler için token sağlayıcıyı ekler
             .AddEntityFrameworkStores<ApplicationDbContext>();
     

        }
    }
}
 