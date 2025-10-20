using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels;


public class ForgetPasswordViewModel
{
    [Required(ErrorMessage = "Email alanı Boş bırakılamaz.")]
    [EmailAddress(ErrorMessage = "Lütfen geçerli bir email adresi giriniz.")]
    [Display(Name = "Email")]
    public string Email { get; set; }

}
