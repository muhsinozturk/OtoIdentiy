using System.Diagnostics;
using System.Security.Claims;
using Application.Abstractions.Services;

using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMvc.Extenisons;
using WebMvc.Models;


namespace WebMvc.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
    
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
          
        }


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AccessDenied(string ReturnUrl)
        {
            string message = string.Empty;

            message = "Bu sayfaya eriþim yetkiniz bulunmamaktadýr. Lütfen yöneticiniz ile iletiþime geçin.";
            ViewBag.message = message;
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
