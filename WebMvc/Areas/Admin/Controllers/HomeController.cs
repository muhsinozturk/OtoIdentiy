using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebMvc.Models;

namespace WebMvc.Areas.Admin.Controllers;

public class HomeController : AdminBaseController
{
   

    public IActionResult Index()
    {
        return View();
    }

  
  
}
