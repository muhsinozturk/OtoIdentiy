

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
using WebMvc.Models.Identity;

namespace WebMvc.Areas.Admin.Controllers;


[Authorize(Roles = "AdminRole,PersonelRole")]
public class MemberController : AdminBaseController
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

        var userViewModel = new UserEditViewModel
        {
            UserName = currentUser.UserName,
            Email = currentUser.Email,
            Phone = currentUser.PhoneNumber,
            BirthDate = currentUser.BirthDate,
            City = currentUser.City,
            PictureFileName = currentUser.Picture // 🔹 Eksik kısım eklendi
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


    [HttpGet]
    public async Task<IActionResult> UserEdit()
    {
        var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

        var userEditViewModel = new UserEditViewModel
        {
            UserName = currentUser!.UserName,
            Email = currentUser.Email,
            Phone = currentUser.PhoneNumber,
            BirthDate = currentUser.BirthDate,
            City = currentUser.City,
            PictureFileName = currentUser.Picture // mevcut resim (örneğin default_user_picture.png)
        };

        return View(userEditViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UserEdit(UserEditViewModel request)
    {
        if (!ModelState.IsValid)
            return View(request);

        var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

        currentUser.UserName = request.UserName;
        currentUser.Email = request.Email;
        currentUser.PhoneNumber = request.Phone;
        currentUser.BirthDate = request.BirthDate;
        currentUser.City = request.City;

        // 🔹 Dosya geldi mi kontrol et
        if (request.Picture != null && request.Picture.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "admin", "img");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Picture.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.Picture.CopyToAsync(fileStream);
            }

            // Eski resmi sil (default hariç)
            if (!string.IsNullOrEmpty(currentUser.Picture) && currentUser.Picture != "user.png")
            {
                var oldFilePath = Path.Combine(uploadsFolder, currentUser.Picture);
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            currentUser.Picture = uniqueFileName;
        }

        var result = await _userManager.UpdateAsync(currentUser);
        if (!result.Succeeded)
        {
            ModelState.AddModelErrorList(result.Errors);
            return View(request);
        }

        await _signInManager.RefreshSignInAsync(currentUser);
        TempData["SuccessMessage"] = "Profil güncelleme başarılı.";

        var updatedVm = new UserEditViewModel
        {
            UserName = currentUser.UserName,
            Email = currentUser.Email,
            Phone = currentUser.PhoneNumber,
            BirthDate = currentUser.BirthDate,
            City = currentUser.City,
            PictureFileName = currentUser.Picture
        };

        return View(updatedVm);
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