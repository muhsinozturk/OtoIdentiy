using System.ComponentModel.DataAnnotations;

namespace WebMvc.Areas.User.Models
{
    public class AdminUserCreateViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress]
        [Display(Name = "E-Posta")]
        public string Email { get; set; } = null!;

        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Rol")]
        public string? RoleName { get; set; }
    }
}
