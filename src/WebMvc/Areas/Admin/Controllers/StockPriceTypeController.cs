using Application.Abstractions.Services;
using Application.DTOs.StockPriceType;
using Microsoft.AspNetCore.Mvc;
using WebMvc.Areas.Admin.Controllers;

namespace WebUI.Areas.Admin.Controllers
{
    public class StockPriceTypeController : UserBaseController
    {
        private readonly IStockPriceTypeService _stockPriceTypeService;
        private readonly ICompanyService _companyService;

        public StockPriceTypeController(
            IStockPriceTypeService stockPriceTypeService,
            ICompanyService companyService)
        {
            _stockPriceTypeService = stockPriceTypeService;
            _companyService = companyService;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _stockPriceTypeService.GetAllAsync();
            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Companies = await _companyService.GetAllAsync();
            return View(new CreateStockPriceTypeDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStockPriceTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync();
                return View(dto);
            }

            await _stockPriceTypeService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var item = (await _stockPriceTypeService.GetAllAsync()).FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();

            ViewBag.Companies = await _companyService.GetAllAsync();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateStockPriceTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync();
                return View(dto);
            }

            await _stockPriceTypeService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var priceType = await _stockPriceTypeService.GetByIdAsync(id);
            if (priceType == null)
            {
                TempData["Error"] = "Fiyat tipi bulunamadı.";
                return RedirectToAction(nameof(Index));
            }

            var relatedStocks = await _stockPriceTypeService.GetRelatedStocksAsync(id);

            var model = new StockPriceTypeDeleteDto
            {
                Id = priceType.Id,
                Name = priceType.Name,
                RelatedStocks = relatedStocks
            };

            return View(model);
        }

        // 🔹 Silme onayı sonrası (POST)
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _stockPriceTypeService.DeleteAsync(id);
            TempData["Success"] = "Fiyat tipi başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
