using Application.Abstractions.Services;
using Application.DTOs.Inventory;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebMvc.Areas.Admin.Controllers;


public class InventoryController : AdminBaseController
{
    private readonly IInventoryService _inventoryService;
    private readonly IStockService _stockService;
    private readonly IStockGroupService _stockGroupService;
    private readonly IDepotService _depotService;

    public InventoryController(
        IDepotService depotService,
        IInventoryService inventoryService,
        IStockService stockService,
        IStockGroupService stockGroupService)
    {
        _depotService = depotService;
        _inventoryService = inventoryService;
        _stockService = stockService;
        _stockGroupService = stockGroupService;
    }

    public async Task<IActionResult> Index(int? depotId)
    {
        var depots = await _depotService.GetAllAsync();
        ViewBag.Depots = new SelectList(depots, "Id", "Name", depotId);

        List<InventoryDto> list = new();
        if (depotId.HasValue)
        {
            list = await _inventoryService.GetByDepotIdAsync(depotId.Value);
            ViewBag.DepotId = depotId.Value;
        }

        return View(list);
    }


    public async Task<IActionResult> Create(int depotId)
    {
        ViewBag.StockGroups = await _stockGroupService.GetAllAsync();
        return View(new CreateInventoryDto { DepotId = depotId });
    }
    [HttpGet]
    public async Task<IActionResult> _CreatePartial(int depotId)
    {
        ViewBag.StockGroups = await _stockGroupService.GetAllAsync();
        return PartialView("_CreatePartial", new CreateInventoryDto { DepotId = depotId });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInventoryDto dto)
    {
        if (!ModelState.IsValid)
        {
            // ViewBag tekrar doldurulmalı, yoksa sayfa hata verir
            ViewBag.StockGroups = await _stockGroupService.GetAllAsync();
            return View(dto);
        }

        try
        {
            await _inventoryService.CreateAsync(dto);
 
            return RedirectToAction("Index", new { depotId = dto.DepotId });
        }
        catch (Exception ex)
        {
            ViewBag.StockGroups = await _stockGroupService.GetAllAsync();
            TempData["Error"] = "Hata: " + ex.Message;
            return View(dto);
        }
    }


    // AJAX
    public async Task<IActionResult> GetStocksByGroup(int groupId)
    {
        var stocks = await _inventoryService.GetByGroupIdAsync(groupId);
        return Json(stocks);
    }
}
