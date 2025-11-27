
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebMvc.Areas.Admin.Controllers;
using WebMvc.Areas.Admin.Models;
using WebMvc.Extenisons;
using WebMvc.Models;


//[Authorize(Roles = "Admin")]
[Area("Admin")]
public class RolesController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles
            .Select(r => new RoleViewModel()
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync();

        return View(roles);
    }

   // [Authorize(Roles ="role-action")]
    public IActionResult RoleCreate()
    {
        return View();
    }
    //[Authorize(Roles = "role-action")]
    [HttpPost]
    public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
    {
        var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });

        if (!result.Succeeded)
        {
            ModelState.AddModelErrorList(result.Errors);
            return View();
        }
        return RedirectToAction(nameof(RolesController.Index));
    }
    public async Task<IActionResult> RoleUpdate(string id)
    {

        var roleToUpdate = await _roleManager.FindByIdAsync(id);

        if (roleToUpdate == null)
        {
            throw new Exception("Güncellenecek Role Bulunamadı");
        }

        return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id, Name = roleToUpdate.Name });
    }

    [HttpPost]
    public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
    {
        var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

        if (roleToUpdate == null)
        {
            throw new Exception("Güncellenecek Role Bulunamadı");
        }
        roleToUpdate.Name = request.Name;
        await _roleManager.UpdateAsync(roleToUpdate);

        ViewData["SuccessMessage"] = "Role Başarıyla Güncellendi";

        return View();
    }

    public async Task<IActionResult> RoleDelete(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);

        if (role == null)
        {
            throw new Exception("Silinecek rol bulunamamıştır");
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            throw new Exception(result.Errors.Select(x => x.Description).First());
        }
        TempData["SuccessMessage"] = "Rol silinmiştir";
        return RedirectToAction(nameof(RolesController.Index));
    }

    public async Task<IActionResult> AssingRoleToUser(string id)
    {
        var currentUser = await _userManager.FindByIdAsync(id);// kullanıcıyı bul
        ViewBag.userId = id;
        var roles = await _roleManager.Roles.ToListAsync();// rollerı listele
        var userRoles = await _userManager.GetRolesAsync(currentUser);// kullanıcının rollerini bul
        var roleViewModelList = new List<AssingRoleToUserViewModel>();// viewmodel listesi oluştur

        foreach (var role in roles)// rollerin içinde dön
        {
            var assignRoleToUserViewModel = new AssingRoleToUserViewModel()// her rol için viewmodel oluştur
            {
                Id = role.Id,
                Name = role.Name,

            };

            if (userRoles.Contains(role.Name))// eğer kullanıcının rolleri arasında bu rol varsa
            {
                assignRoleToUserViewModel.Exist = true;//
            }
            roleViewModelList.Add(assignRoleToUserViewModel);

          ;
        }
        return View(roleViewModelList);// viewmodel listesini viewa gönder
    }

    [HttpPost]
    public async Task<IActionResult> AssingRoleToUser(string userId ,List<AssingRoleToUserViewModel>requestList)
    {
       var userToAssignRoles= await _userManager.FindByIdAsync(userId);

        foreach (var role in requestList)
        {
            if (role.Exist)// eğer checkbox işaretli ise
            {
                await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);// kullanıcıya rol ekle
            }
            else
            {
                await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);// kullanıcıdan rol çıkar
            }
        }
        return RedirectToAction(nameof(HomeController.UserList), "Home");
    }

}
