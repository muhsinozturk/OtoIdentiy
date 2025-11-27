using System.ComponentModel.DataAnnotations;

namespace WebMvc.Models.Identity;

public class PasswordChangeViewModel
{

    [DataType(DataType.Password)]
    [Display(Name = "Eski Şifre")]
    [Required(ErrorMessage = "Eski Şifre boş bırakılamaz")]
    [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
    public string PasswordOld { get; set; }


    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre")]
    [Required(ErrorMessage = "Yeni Şifre boş bırakılamaz")]
    [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
    public string PasswordNew { get; set; }

    [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
    [DataType(DataType.Password)]   
    [Compare(nameof(PasswordNew), ErrorMessage = "Şifreler uyuşmuyor")]
    [Display(Name = "Yeni Şifre Tekrar")]
    public string PasswordConfirm { get; set; }
}
