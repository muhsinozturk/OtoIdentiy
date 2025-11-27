using Application.Abstractions.Services;
using Application.DTOs.Vehicle;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Areas.User.Controllers;

public class VehicleController : UserBaseController
{
    private readonly IVehicleService _vehicleService;
    private readonly IActService _actService;

    public VehicleController(IVehicleService vehicleService, IActService actService)
    {
        _vehicleService = vehicleService;
        _actService = actService;
    }
    public async Task<IActionResult> Index(int? actId, string? search, int page = 1, int pageSize = 10)
    {
        var vehicles = actId.HasValue && actId > 0
            ? await _vehicleService.GetAllByActIdAsync(actId.Value)
            : await _vehicleService.GetAllAsync();

        // Arama filtresi
        if (!string.IsNullOrEmpty(search))
            vehicles = vehicles.Where(v => v.Plate.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        // Sayfalama
        var totalCount = vehicles.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pagedVehicles = vehicles.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // ViewBag bilgileri
        ViewBag.ActId = actId;
        ViewBag.Search = search;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalCount = totalCount;
        ViewBag.PageSize = pageSize;

        if (actId.HasValue && actId > 0)
        {
            var act = await _actService.GetByIdAsync(actId.Value);
            ViewBag.ActName = act?.FullName;
        }
        else
        {
            ViewBag.ActName = "Tüm Araçlar";
        }

        return View(pagedVehicles);
    }


    [HttpGet]
    public async Task<IActionResult> Create(int actId)
    {
        var act = await _actService.GetByIdAsync(actId);
        if (act == null) return NotFound();

        ViewBag.Act = act;
        return View(new CreateVehicleDto { ActId = actId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateVehicleDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Act = await _actService.GetByIdAsync(dto.ActId);
            return View(dto);
        }

        await _vehicleService.CreateAsync(dto);
        TempData["Success"] = "Araç başarıyla eklendi";
        return RedirectToAction("Index", "Act", new { area = "User", id = dto.ActId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);
        if (vehicle == null) return NotFound();

        return View(vehicle);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(VehicleDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        await _vehicleService.UpdateAsync(dto);
        return RedirectToAction("Index", "Act", new { area = "User", id = dto.ActId });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);
        if (vehicle == null) return NotFound();

        return View(vehicle);
    }

    [HttpPost]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var vehicle = await _vehicleService.GetByIdAsync(id);
        if (vehicle == null) return NotFound();

        await _vehicleService.DeleteAsync(id);
        return RedirectToAction("Index", "Act", new { area = "User", id = vehicle.ActId });
    }
}
