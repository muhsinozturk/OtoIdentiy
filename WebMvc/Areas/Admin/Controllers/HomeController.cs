
using Application.ViewModels;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMvc.Models;

namespace WebMvc.Areas.Admin.Controllers;


public class HomeController : AdminBaseController
{
    private readonly UserManager<AppUser> _userManager;

    public HomeController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> UserList()
    {
        var userList = await _userManager.Users.ToListAsync();

        var userViewModelList = userList.Select(x => new UserViewModel()
        {
            Id = x.Id,
            UserName = x.UserName,
            Email = x.Email
        }).ToList();
        return View(userViewModelList);

    }

}
