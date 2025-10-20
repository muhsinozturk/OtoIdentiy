using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels;


public class ResetPasswordViewModel
{
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]

    [Required(ErrorMessage = "Şifre boş bırakılamaz")]
    public string Password { get; set; }



    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifreler uyuşmuyor")]    
    [Display(Name = "Şifre tekrar")]

    [Required(ErrorMessage = "Şifreler uyuşmuyor")]
    public string PasswordConfirm { get; set; }
}
