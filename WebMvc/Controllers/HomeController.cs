using System.Diagnostics;
using System.Security.Claims;
using Application.Abstractions.Services;
using Application.ViewModels;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)//modelde hata varsa
            {
                return View();
            }
            //kullanýcý adý varmý
            var identiyResult = await _userManager.CreateAsync(new()
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.Phone
            }, request.Password);

            if (!identiyResult.Succeeded)
            {
                ModelState.AddModelErrorList(identiyResult.Errors.Select(x => x.Description).ToList());
                return View();
            }

            //kullanýcý oluþturulduktan sonra claim ekleme 
            var exchangeClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
            //claim ekleme
            var user = await _userManager.FindByNameAsync(request.UserName);
            //claim ekleme
            var claimResult = await _userManager.AddClaimAsync(user!, exchangeClaim);
            //claim ekleme kontrol
            if (!claimResult.Succeeded)
            {
                ModelState.AddModelErrorList(claimResult.Errors.Select(x => x.Description).ToList());
                return View();//
            }

            TempData["SuccessMessage"] = "Kullanýcý kaydý baþarýlý";
            return RedirectToAction("SignUp", "Home");





        }


        public IActionResult SignIn()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model, string? returnUrl = null)
        {
            returnUrl = returnUrl ??= Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(model.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Kullanýcý veya þifre yanlýþ");

                return View();
            }



            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, model.Password, model.RememberMe, true); //buradaki true hesabý kilitler dk ve 3 kez yanlýþ girersen mesela)

            if (signInResult.IsLockedOut)
            {
                ModelState.AddModelErrorList(new List<string>() { "Hesabýnýz 3dk kilitlenmiþtir.Lütfen daha sonra tekrar deneyiniz." });
                return View();
            }

            if (!signInResult.Succeeded)
            {
                ModelState.AddModelErrorList(new List<string>() { $"email veya þifre yanlýþ(Baþarýsýz giriþ sayýsý = {await _userManager.GetAccessFailedCountAsync(hasUser)})" });
                return View();
            }


            if (hasUser.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(hasUser, model.RememberMe, new[] {
                    new Claim("birthdate", hasUser.BirthDate.Value.ToString()) });

            }

            return Redirect(returnUrl);








        }


        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {

            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Bu e-maile sahipkullanýcý bulunamadý");
                return View();
            }

            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser); //þifre sýfýrlama tokený oluþturur
            var passwordResetLink = Url.Action("ResetPassword", "Home", new
            {
                userId = hasUser.Id,
                token = resetToken
            }, HttpContext.Request.Scheme);


            //örnek link http://localhost:7089/userId=1234&token=asdasdasd

            await _emailService.SendResetPasswordEmail(passwordResetLink, hasUser.Email);

            TempData["SuccessMessage"] = "Þifre sýfýrlama linki e posta adresine gönderilmiþtir.";
            return RedirectToAction(nameof(ForgetPassword));

        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel request)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("Bir hata oluþtu");
            }

            var hasUser = await _userManager.FindByIdAsync(userId!.ToString());

            if (hasUser == null)
            {
                ModelState.AddModelError(String.Empty, "Kullanýcý bulunamadý");
                return View();
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(hasUser, token!.ToString(), request.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Þifre sýfýrlama iþlemi baþarýlý";

            }

            else
            {
                ModelState.AddModelErrorList(result.Errors.Select(x => x.Description).ToList());

            }

            return View();
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
