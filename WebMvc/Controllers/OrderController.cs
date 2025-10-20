using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Controllers
{

    public class OrderController : Controller
    {
        [Authorize(Policy = "Permissions.Order.Read")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
