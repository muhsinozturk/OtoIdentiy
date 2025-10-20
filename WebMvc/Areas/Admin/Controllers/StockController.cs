using Application.Abstractions.Services;
using Application.DTOs.Company;
using Application.DTOs.Stock;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Areas.Admin.Controllers
{

    public class StockController : AdminBaseController
    {
        private readonly IStockService _stockService;
        private readonly ICompanyService _companyService;
        private readonly IStockGroupService _stockGroupService;

        public StockController(IStockService stockService, ICompanyService companyService, IStockGroupService stockGroupService)
        {
            _stockService = stockService;
            _companyService = companyService;
            _stockGroupService = stockGroupService;
        }

        public async Task<IActionResult> Index(int? groupId)
        {
            if (groupId.HasValue)
            {
                var group = await _stockGroupService.GetByIdAsync(groupId.Value);
                ViewBag.GroupId = groupId;
                ViewBag.GroupName = group?.Name ?? "Belirtilmemiş Grup";

                var stocks = await _stockService.GetByGroupIdAsync(groupId.Value); // ✅ artık var
                return View(stocks);
            }

            var allStocks = await _stockService.GetAllAsync();
            return View(allStocks);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Companies = await _companyService.GetAllAsync();
            ViewBag.Groups = await _stockGroupService.GetAllAsync(); // ✅ gruplar eklendi
            return View(new CreateStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStockDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync();
                ViewBag.Groups = await _stockGroupService.GetAllAsync(); // hata olursa tekrar doldur
                return View(dto);
            }

            await _stockService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var stock = await _stockService.GetByIdAsync(id);
            if (stock == null) return NotFound();
            return View(stock);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var stock = await _stockService.GetByIdForEditAsync(id);
            if (stock == null) return NotFound();

            ViewBag.Companies = await _companyService.GetAllAsync();
            ViewBag.Groups = await _stockGroupService.GetAllAsync();

            return View(stock); // ✅ artık EditStockDto geliyor
        }



        [HttpPost]
        public async Task<IActionResult> Edit(EditStockDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync();
                ViewBag.Groups = await _stockGroupService.GetAllAsync();
                return View(dto);
            }

            await _stockService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Delete(int id)
        {
            var stock = await _stockService.GetByIdAsync(id);
            if (stock == null) return NotFound();
            return View(stock);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _stockService.DeleteAsync(id);
            TempData["Success"] = "Stok başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
