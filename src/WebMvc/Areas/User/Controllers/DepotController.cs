using Application.Abstractions.Services;
using Application.DTOs.Depot;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Areas.User.Controllers
{

    public class DepotController : UserBaseController
    {
        private readonly IDepotService _depotService;
        private readonly ICompanyService _companyService;
        private readonly IInventoryService _inventoryService;
        private readonly IStockGroupService _stockGroupService;
        public DepotController(IDepotService depotService, ICompanyService companyService, IInventoryService inventoryService, IStockGroupService stockGroupService)
        {
            _depotService = depotService;
            _companyService = companyService;
            _inventoryService = inventoryService;
            _stockGroupService = stockGroupService;
        }

        public async Task<IActionResult> Index()
        {
            var depots = await _depotService.GetAllAsync();
            return View(depots);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Companies = await _companyService.GetAllAsync(); // Şirket listesi dolduruluyor
            return View(new CreateDepotDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDepotDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync(); // Hata varsa liste yine doldurulmalı
                return View(dto);
            }

            await _depotService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var depot = await _depotService.GetByIdAsync(id);
            if (depot == null) return NotFound();

            ViewBag.Companies = await _companyService.GetAllAsync(); // Depo düzenlerken de şirket listesi lazım
            return View(depot);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditDepotDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync();
                return View(dto);
            }

            await _depotService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var depot = await _depotService.GetByIdAsync(id);
            if (depot == null) return NotFound();
            return View(depot);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _depotService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var depot = await _depotService.GetByIdAsync(id);
            var inventory = await _inventoryService.GetDepotSummaryAsync(id);

            ViewBag.Depot = depot;
            ViewBag.StockGroups = await _stockGroupService.GetAllAsync(); // ✅ önemli
            return View(inventory);
        }


    }
}
