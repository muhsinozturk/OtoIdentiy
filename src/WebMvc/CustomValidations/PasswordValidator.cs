using WebMvc.Models;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;

namespace WebMvc.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {

            var errors = new List<IdentityError>();
            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new() { Code="PasswordContainsUserName", Description="Şifre kullanıcı adını içeremez" });

            }

            if(password!.ToLower().StartsWith("1234"))
            {
                errors.Add(new() { Code = "PasswordStartsWith1234", Description = "Şifre 1234 ile başlayamaz" });
            }

            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray())); //hata mesajlarını gösterecek burası örneğin şifrede kullanıcı adı kullanılamaz
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
//ToLower = büyük küçük karakter duyarlılığını ortadan kaldırır.