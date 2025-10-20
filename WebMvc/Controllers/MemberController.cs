
using Application.ViewModels;

using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Claims;
using WebMvc.Extenisons;
using WebMvc.Models;

namespace WebMvc.Controllers;


[Authorize]
public class MemberController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IFileProvider _fileProvider;
    public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IFileProvider fileProvider)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _fileProvider = fileProvider;
    }



    public async Task<IActionResult> Index()
    {

        var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
        var userViewModel = new UserViewModel()
        {
            UserName = currentUser!.UserName,
            Email = currentUser!.Email,
            Phone = currentUser!.PhoneNumber,
            PictureUrl = currentUser!.Picture
        };
        return View(userViewModel);
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();

    }

    public async Task<IActionResult> PasswordChange()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> PasswordChange(PasswordChangeViewModel request)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

        var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser!, request.PasswordOld);
        if (!checkOldPassword)
        {
            ModelState.AddModelError(string.Empty, "Eski şifre yanlış");
            return View();
        }

        var identityResult = await _userManager.ChangePasswordAsync(currentUser!, request.PasswordOld, request.PasswordNew);

        if (!identityResult.Succeeded)
        {
            ModelState.AddModelErrorList(identityResult.Errors.Select(x => x.Description).ToList());
            return View();
        }

        await _userManager.UpdateSecurityStampAsync(currentUser!); //kullanıcının güvenlik damgasını günceller, böylece tüm aktif oturumları geçersiz kılar
        await _signInManager.SignOutAsync();//kullanıcıyı oturumdan çıkarır
        await _signInManager.PasswordSignInAsync(currentUser!, request.PasswordNew, true, false);//kullanıcıyı yeni şifresiyle tekrar oturum açtırır

        TempData["SuccessMessage"] = "Şifre değişikliği başarılı, lütfen tekrar giriş yapın";
        return View();
    }


    public async Task<IActionResult> UserEdit()
    {

        ViewBag.gender = new SelectList(Enum.GetNames(typeof(Gender)));
        var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

        var userEditViewModel = new UserEditViewModel()
        {
            UserName = currentUser.UserName,
            Email = currentUser.Email,
            Phone = currentUser.PhoneNumber,
            BirthDate = currentUser.BirthDate,
            City = currentUser.City,
            Gender = currentUser.Gender,
        };


        return View(userEditViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UserEdit(UserEditViewModel request)
    {
        if (!ModelState.IsValid)
        {
            return View();

        }

        var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;
        currentUser.UserName = request.UserName;
        currentUser.Email = request.Email;
        currentUser.PhoneNumber = request.Phone;
        currentUser.BirthDate = request.BirthDate;
        currentUser.City = request.City;
        currentUser.Gender = request.Gender;


        if (request.Picture != null && request.Picture.Length > 0)
        {
            var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
            string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

            var newPicturePath = Path.Combine(wwwrootFolder!.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);
            using var stream = new FileStream(newPicturePath, FileMode.Create);
            await request.Picture.CopyToAsync(stream);
            currentUser.Picture = randomFileName;

        }
        var updateToUserResult = await _userManager.UpdateAsync(currentUser);
        if (!updateToUserResult.Succeeded)
        {
            ModelState.AddModelErrorList(updateToUserResult.Errors);
            return View();
        }


        await _userManager.UpdateSecurityStampAsync(currentUser);
        await _signInManager.SignOutAsync();

        if(request.BirthDate.HasValue)
        {
            await _signInManager.SignInWithClaimsAsync(currentUser, true, new[] {
                    new Claim("birthdate", currentUser.BirthDate!.Value.ToString()) });
        }
        else
        {
            await _signInManager.SignInAsync(currentUser, true);
        }
   
        
    

        TempData["SuccessMessage"] = "Profil güncelleme başarılı";

        var userEditViewModel = new UserEditViewModel()
        {
            UserName = currentUser.UserName,
            Email = currentUser.Email,
            Phone = currentUser.PhoneNumber,
            BirthDate = currentUser.BirthDate,
            City = currentUser.City,
            Gender = currentUser.Gender,

        };

        return View(userEditViewModel);



    }

    public IActionResult AccessDenied(string ReturnUrl)
    {
        string message = string.Empty;

        message = "Bu sayfaya erişim yetkiniz bulunmamaktadır. Lütfen yöneticiniz ile iletişime geçin.";
        ViewBag.message = message;
        return View();
    }

    public IActionResult Claims()
    {
        var userClaimList = User.Claims.Select(c => new ClaimViewModel()
        {
            Issuer = c.Issuer,
            Type = c.Type,
            Value = c.Value
        }).ToList();
        return View(userClaimList);
    }

    [Authorize(Policy = "AnkaraPolicy")]
    public IActionResult AnkaraPage()
    {
        return View();
    }


    [Authorize(Policy = "ExchangePolicy")]
    public IActionResult ExchangePage()
    {
        return View();
    }

    [Authorize(Policy = "ViolencePolicy")]
    public IActionResult ViolencePage()
    {
        return View();
    }
}