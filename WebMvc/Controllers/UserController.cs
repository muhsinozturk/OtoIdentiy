using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers;

public class UserController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    // GET: /User/Login
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    // POST: /User/Login
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        // RememberMe'yi kapattık (daima false)
        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            false,  // 🔹 RememberMe devre dışı
            false
        );

        if (result.Succeeded)
        {
            return Redirect(returnUrl ?? Url.Action("Index", "Home", new { area = "Admin" })!);
        }

        ModelState.AddModelError("", "Geçersiz giriş.");
        return View(model);
    }

    // GET: /User/Logout
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "User");
    }
}
