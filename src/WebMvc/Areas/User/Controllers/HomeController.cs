using Application.Abstractions.Services;

using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMvc.Models.Identity;


namespace WebMvc.Areas.User.Controllers;


public class HomeController : UserBaseController
{
    private readonly IWorkOrderService _workOrderService;
    private readonly IInvoiceService _invoiceService;
    private readonly IEmployeeService _employeeService;
    private readonly IActService _actService;
    private readonly IVehicleService _vehicleService;
    private readonly IExternalApiService _externalApiService;
    private readonly UserManager<AppUser> _userManager;

    public HomeController(
        IWorkOrderService workOrderService,
        IInvoiceService invoiceService,
        IEmployeeService employeeService,
        IActService actService,
        IVehicleService vehicleService,
        UserManager<AppUser> userManager,
        IExternalApiService externalApiService)
    {
        _workOrderService = workOrderService;
        _invoiceService = invoiceService;
        _employeeService = employeeService;
        _actService = actService;
        _vehicleService = vehicleService;
        _userManager = userManager;
        _externalApiService = externalApiService;
    }

  
  //  [Authorize(Roles = "AdminRole,PersonelRole")]
    public async Task<IActionResult> Index()
    {
        var workOrders = await _workOrderService.GetAllAsync();
        var invoices = await _invoiceService.GetAllAsync();
        var employees = await _employeeService.GetAllAsync();
        var acts = await _actService.GetAllAsync();
        var vehicles = await _vehicleService.GetAllAsync();

        var totalRevenue = invoices.Sum(i => i.Total);
        var todayRevenue = invoices.Where(i => i.Date.Date == DateTime.Today).Sum(i => i.Total);

        var monthlyRevenue = invoices
            .GroupBy(i => new { i.Date.Year, i.Date.Month })
            .Select(g => new { Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM"), Total = g.Sum(x => x.Total) })
            .OrderBy(x => x.Month)
            .ToList();

        // 🔹 API verileri
        var currency = await _externalApiService.GetCurrencyRatesAsync();
     

        ViewBag.Currency = currency;
   
        ViewBag.MonthlyRevenue = monthlyRevenue;
        ViewBag.TotalWorkOrders = workOrders.Count;
        ViewBag.TotalRevenue = totalRevenue;
        ViewBag.TodayRevenue = todayRevenue;
        ViewBag.EmployeeCount = employees.Count;
        ViewBag.CustomerCount = acts.Count;
        ViewBag.VehicleCount = vehicles.Count;

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
