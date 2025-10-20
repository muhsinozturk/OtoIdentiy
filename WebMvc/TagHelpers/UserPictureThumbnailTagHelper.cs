using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebMvc.TagHelpers;

public class UserPictureThumbnailTagHelper : TagHelper
{
    public string? PictureUrl { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "img"; // etiket img olacak
        output.TagMode = TagMode.SelfClosing; // etiket <img /> şeklinde kapanacak

        if (string.IsNullOrEmpty(PictureUrl))
        {
            output.Attributes.SetAttribute("src","/userpictures/default_user_picture.png");
        }
        else
        {
            output.Attributes.SetAttribute("src",$"/userpictures/{PictureUrl}");
        }
    }
}
