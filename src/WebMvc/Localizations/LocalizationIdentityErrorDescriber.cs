using Microsoft.AspNetCore.Identity;

namespace WebMvc.Localizations
{
    public class LocalizationIdentityErrorDescriber: IdentityErrorDescriber
    {
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"Kullanıcı adı '{userName}' zaten kullanımda."
            };
        }
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"Kullanıcı adı '{email}' zaten kullanımda."
            };
        }
   


    }
}
