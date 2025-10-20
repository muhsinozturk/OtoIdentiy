using WebMvc.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Infrastructure.Identity;

namespace WebMvc.ClaimProvider;

public class UserClaimProvider : IClaimsTransformation
{
    private readonly UserManager<AppUser> _userManager;

    public UserClaimProvider(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identityUser = principal.Identity as ClaimsIdentity;//kullanıcının kimlik bilgilerini al
        var currentUser = await _userManager.FindByNameAsync(identityUser!.Name!); //kullanıcı adını kullanarak kullanıcıyı bul

        if (currentUser == null)//eğer kullanıcı bulunamazsa
        { 
        
            return principal;

        }

        if (String.IsNullOrEmpty(currentUser!.City)) //eğer kullanıcının şehir bilgisi boşsa
        {
            return principal;
        }
        

        if (!principal.HasClaim(c => c.Type == "city")) //eğer kullanıcının "city" claimi yoksa
        {
            Claim cityClaim= new Claim("city", currentUser.City);//yeni bir claim oluştur
            identityUser.AddClaim(cityClaim);//ve bu claimi kullanıcının kimlik bilgilerine ekle
        }

        

        return principal;
    }
}
