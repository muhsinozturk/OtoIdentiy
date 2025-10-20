using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebMvc.Requirements;

public class ExchangeExpireRequirement : IAuthorizationRequirement
{
}
//bir kullanıcı bir sayfaya belirli bir tarihten sonra erişemesin

public class ExchangeExpireRequirementHandler : AuthorizationHandler<ExchangeExpireRequirement>//
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExchangeExpireRequirement requirement)
    {
       
        if (!context.User.HasClaim(x => x.Type == "ExchangeExpireDate")) //
        {
         context.Fail();// claim yoksa erişim izni verme
            return Task.CompletedTask; // işlemi tamamla
        }

        Claim exchangeExpireDate = context.User.FindFirst("ExchangeExpireDate")!; //
        
        if(DateTime.Now> Convert.ToDateTime(exchangeExpireDate.Value))  //
        {
            context.Fail();
            return Task.CompletedTask;
        }
      
        context.Succeed(requirement);
        return Task.CompletedTask;
    }
} 