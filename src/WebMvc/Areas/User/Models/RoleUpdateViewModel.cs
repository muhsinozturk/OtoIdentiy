using System.ComponentModel.DataAnnotations;

namespace WebMvc.Areas.User.Models
{
    public class RoleUpdateViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Role ismi boş bırakılamaz")]
        [Display(Name = "Role İsmi")]
        public string Name { get; set; } = null!;
    }
}
