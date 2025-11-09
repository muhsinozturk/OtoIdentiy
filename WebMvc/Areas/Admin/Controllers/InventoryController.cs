using Application.Abstractions.Services;
using Application.DTOs.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebMvc.Areas.Admin.Controllers;

namespace WebMvc.Areas.Admin.Controllers
{
    [Area("Admin")]
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

        // 📦 INDEX – Depo seçimi + filtre + sayfalama + giriş/çıkış ayrımı
        public async Task<IActionResult> Index(int? depotId, string? search, DateTime? startDate, DateTime? endDate, int page = 1)
        {
            const int pageSize = 10;

            var depots = await _depotService.GetAllAsync();
            ViewBag.Depots = new SelectList(depots, "Id", "Name", depotId);

            // 🔹 Depo seçilmediyse boş görünüm
            if (!depotId.HasValue)
                return View(new List<DepotInventorySummaryDto>());

            // 🔹 1. Depo özeti (stok bazlı toplamlar)
            var summary = await _inventoryService.GetDepotSummaryAsync(depotId.Value);

            // 🔹 2. Detaylı hareket listesi (filtreli)
            var allInventories = await _inventoryService.GetByFilterAsync(depotId.Value, startDate, endDate, null, search);

            // 🔹 Sayfalama işlemi
            var totalCount = allInventories.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            var pagedList = allInventories
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 🔹 Giriş & çıkış listeleri
            ViewBag.Inputs = allInventories
                .Where(x => x.IsInput)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            ViewBag.Outputs = allInventories
                .Where(x => !x.IsInput)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            // 🔹 ViewBag bilgileri
            ViewBag.DepotId = depotId;
            ViewBag.DepotName = depots.FirstOrDefault(x => x.Id == depotId)?.Name ?? "";
            ViewBag.Search = search;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // 📤 Model = stok özeti (DepotInventorySummaryDto)
            return View(summary);
        }

        // 📥 YENİ ENVANTER EKLEME (giriş işlemi)
        [HttpGet]
        public async Task<IActionResult> Create(int depotId)
        {
            ViewBag.StockGroups = await _stockGroupService.GetAllAsync();
            return View(new CreateInventoryDto { DepotId = depotId, IsInput = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInventoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.StockGroups = await _stockGroupService.GetAllAsync();
                return View(dto);
            }

            try
            {
                await _inventoryService.CreateAsync(dto);
                TempData["Success"] = "Stok hareketi başarıyla eklendi.";
                return RedirectToAction("Index", new { depotId = dto.DepotId });
            }
            catch (Exception ex)
            {
                ViewBag.StockGroups = await _stockGroupService.GetAllAsync();
                TempData["Error"] = "Hata: " + ex.Message;
                return View(dto);
            }
        }

        // 📄 Partial (modal içi hızlı ekleme)
        [HttpGet]
        public async Task<IActionResult> _CreatePartial(int depotId)
        {
            ViewBag.StockGroups = await _stockGroupService.GetAllAsync();
            return PartialView("_CreatePartial", new CreateInventoryDto { DepotId = depotId, IsInput = true });
        }

        // 📦 AJAX – Grup bazlı stok listesi (dropdown)
        [HttpGet]
        public async Task<IActionResult> GetStocksByGroup(int groupId)
        {
            var stocks = await _stockService.GetByGroupIdAsync(groupId);
            return Json(stocks);
        }
    }
}
