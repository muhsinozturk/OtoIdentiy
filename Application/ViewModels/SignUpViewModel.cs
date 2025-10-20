using System.ComponentModel.DataAnnotations;
namespace Application.ViewModels;


public class SignUpViewModel
{
    public SignUpViewModel()
    {

    }
    public SignUpViewModel(string userName, string email, string phone, string password)
    {
        UserName = userName;
        Email = email;
        Phone = phone;
        Password = password;
    }

    [Required(ErrorMessage ="Kullanıcı adı boş")]
    [Display(Name = "Kullanıcı Adı")]
    public string UserName { get; set; }


    [Display(Name = "Email Adresi")]
    [Required(ErrorMessage = "Email Formatı yanlış")]
    public string Email { get; set; }


    [Display(Name = "Telefon Numarası")]
    public string Phone { get; set; }



    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
    [Required(ErrorMessage = "Şifre boş bırakılamaz")]
    public string Password { get; set; }


    [DataType(DataType.Password)]
    [Display(Name = "Şifre tekrar")]
    [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
    [Required(ErrorMessage = "Şifreler uyuşmuyor")]
    public string PasswordConfirm { get; set; }
}
