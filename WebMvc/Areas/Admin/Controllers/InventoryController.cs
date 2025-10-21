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

    public async Task<IActionResult> Index(int? depotId, string? search, DateTime? startDate, DateTime? endDate, int page = 1, int pageSize = 10)
    {
        // Depolar
        var depots = await _depotService.GetAllAsync();
        ViewBag.Depots = new SelectList(depots, "Id", "Name", depotId);

        List<InventoryDto> inventories = new();

        if (depotId.HasValue)
        {
            inventories = await _inventoryService.GetByDepotIdAsync(depotId.Value);

            // Tarih aralığı filtresi
            if (startDate.HasValue && endDate.HasValue)
            {
                inventories = inventories
                    .Where(x => x.CreatedAt.Date >= startDate.Value.Date && x.CreatedAt.Date <= endDate.Value.Date)
                    .ToList();
            }
            else if (startDate.HasValue) // sadece başlangıç verilmişse
            {
                inventories = inventories
                    .Where(x => x.CreatedAt.Date >= startDate.Value.Date)
                    .ToList();
            }
            else if (endDate.HasValue) // sadece bitiş verilmişse
            {
                inventories = inventories
                    .Where(x => x.CreatedAt.Date <= endDate.Value.Date)
                    .ToList();
            }

            // Arama filtresi
            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.ToLower();
                inventories = inventories.Where(x =>
                    (x.StockDto.Name?.ToLower().Contains(term) ?? false) ||
                    (x.StockDto.Code?.ToLower().Contains(term) ?? false) ||
                    (x.StockDto.Brand?.ToLower().Contains(term) ?? false) ||
                    (x.StockDto.Model?.ToLower().Contains(term) ?? false)
                ).ToList();
            }

            // Sayfalama
            var totalCount = inventories.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var paged = inventories.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // ViewBag
            ViewBag.DepotId = depotId;
            ViewBag.Search = search;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageSize = pageSize;

            return View(paged);
        }

        ViewBag.DepotId = null;
        return View(inventories);
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
