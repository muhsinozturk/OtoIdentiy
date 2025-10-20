using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels

{
    public class SignInViewModel
    {

        [Required(ErrorMessage = "Email alanı Boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir email adresi giriniz.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Email alanı Boş bırakılamaz.")]
        [Display(Name = "Şifre")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
        public string Password { get; set; } 


        public bool RememberMe { get; set; }

    }
}
