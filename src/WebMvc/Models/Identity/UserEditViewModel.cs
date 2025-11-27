namespace WebMvc.Models.Identity;


using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


public class UserEditViewModel
{
    [Required(ErrorMessage = "Kullanıcı adı boş")]
    [Display(Name = "Kullanıcı Adı")]
    public string UserName { get; set; }

    [Display(Name = "Email Adresi")]
    [Required(ErrorMessage = "Email Formatı yanlış")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Telefon boş olamaz")]
    [Display(Name = "Telefon Numarası")]
    public string Phone { get; set; } = null!;

    [DataType(DataType.Date)]
    [Display(Name = "Doğum Tarihi:")]
    public DateTime? BirthDate { get; set; }

    [Display(Name = "Şehir:")]
    public string? City { get; set; }

    [Display(Name = "Profil Resmi :")]
    public IFormFile? Picture { get; set; }

    // 🔹 Eklenecek alan
    public string? PictureFileName { get; set; } // Mevcut profil resminin dosya adı
}
