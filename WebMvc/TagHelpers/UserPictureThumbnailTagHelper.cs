using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebMvc.TagHelpers;

public class UserPictureThumbnailTagHelper : TagHelper
{
    // View'dan picture-url parametresi gelecek
    public string? PictureFileName { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "img"; // <img> etiketi oluştur
        output.TagMode = TagMode.SelfClosing; // <img /> şeklinde kapat

        // 🔹 Eğer kullanıcı resmi yoksa default görseli kullan
        if (string.IsNullOrEmpty(PictureFileName))
        {
            output.Attributes.SetAttribute("src", "/company/img/user.png");
        }
        else
        {
            output.Attributes.SetAttribute("src", $"/company/img/{PictureFileName}");
        }

        // Opsiyonel: varsayılan class ve style
        if (!output.Attributes.ContainsName("class"))
            output.Attributes.SetAttribute("class", "img-fluid rounded-circle");

        if (!output.Attributes.ContainsName("style"))
            output.Attributes.SetAttribute("style", "width:150px;height:150px;object-fit:cover;");
    }
}
