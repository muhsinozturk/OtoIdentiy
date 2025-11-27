using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebMvc.Areas.Admin.Models;


namespace WebMvc.Areas.Admin.Controllers;


public class UserController : UserBaseController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // 🔹 Kullanıcı listesi
    public IActionResult Index()
    {
        var users = _userManager.Users
            .Select(x => new UserViewModel
            {
                Id = x.Id,
                Name = x.UserName,
                Email = x.Email
            }).ToList();

        return View(users);
    }

    // 🔹 Kullanıcı oluşturma formu (GET)
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        return View();
    }

    // 🔹 Kullanıcı oluşturma (POST)
    [HttpPost]
    public async Task<IActionResult> Create(AdminUserCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(model);
        }

        var user = new AppUser
        {
            UserName = model.UserName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(model);
        }

        // 🔹 Rol atama
        if (!string.IsNullOrEmpty(model.RoleName))
            await _userManager.AddToRoleAsync(user, model.RoleName);

        TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var roles = await _roleManager.Roles.ToListAsync();
        var userRoles = await _userManager.GetRolesAsync(user);

        var model = new AdminUserEditViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            RoleName = userRoles.FirstOrDefault() // mevcut rolü getir
        };

        ViewBag.Roles = new SelectList(roles, "Name", "Name");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(AdminUserEditViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(model);
        }

        user.UserName = model.UserName;
        user.Email = model.Email;
        user.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(model);
        }

        // 🔹 Rol güncelle
        var oldRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, oldRoles);
        if (!string.IsNullOrEmpty(model.RoleName))
            await _userManager.AddToRoleAsync(user, model.RoleName);

        TempData["SuccessMessage"] = "Kullanıcı bilgileri güncellendi.";
        return RedirectToAction(nameof(Index));
    }

}
