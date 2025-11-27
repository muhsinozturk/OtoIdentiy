using WebMvc.Models;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;

namespace WebMvc.CustomValidations;

public class UserValidator : IUserValidator<AppUser>
{
    public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
    {
        var errors = new List<IdentityError>();

        if (user.UserName.ToLower().Contains("admin"))
        {
            errors.Add(new() { Code = "UserNameContainsAdmin", Description = "Kullanıcı adı admin içeremez" });
        }


        if (user.UserName!.ToLower().StartsWith("1234"))
        {
            errors.Add(new() { Code = "UserNameStartsWith1234", Description = "Kullanıcı adı 1234 ile başlayamaz" });
        }


        if (errors.Any())
        {
            return Task.FromResult(IdentityResult.Failed(errors.ToArray())); //hata mesajlarını gösterecek burası örneğin şifrede kullanıcı adı kullanılamaz
        }
        return Task.FromResult(IdentityResult.Success);
    }
}
