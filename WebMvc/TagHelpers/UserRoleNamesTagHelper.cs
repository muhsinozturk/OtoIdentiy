
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;
using WebMvc.Models;

namespace WebMvc.TagHelpers;


public class UserRoleNamesTagHelper : TagHelper
{
    public string UserId { get; set; }
    private readonly UserManager<AppUser> _userManager;

    public UserRoleNamesTagHelper(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      var user = await _userManager.FindByIdAsync(UserId);
        var roles = await _userManager.GetRolesAsync(user);
        var strinBuilder = new StringBuilder();
        roles.ToList().ForEach(r =>
        { strinBuilder.Append(@$"<span class='badge bg-secondary'>{r.ToLower()}</span> ");
        });
        output.Content.SetHtmlContent(strinBuilder.ToString());
    

    }
}