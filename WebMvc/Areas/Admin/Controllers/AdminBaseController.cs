using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin,PersonelRole")]
public class AdminBaseController : Controller
{


}
