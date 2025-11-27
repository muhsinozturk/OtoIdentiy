using Application.Abstractions.Services;
using Application.DTOs.StockPrice;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Areas.Admin.Controllers
{
   
    public class StockPriceController : UserBaseController
    {
        private readonly IStockPriceService _service;
        private readonly IStockService _stockService;
        private readonly IStockPriceTypeService _typeService;

        public StockPriceController(
            IStockPriceService service,
            IStockService stockService,
            IStockPriceTypeService typeService)
        {
            _service = service;
            _stockService = stockService;
            _typeService = typeService;
        }

        public async Task<IActionResult> Index(int stockId)
        {
            var stock = await _stockService.GetByIdAsync(stockId);
            if (stock == null) return NotFound(); // güvenlik

            ViewBag.Stock = stock;
            var prices = await _service.GetByStockIdAsync(stockId);
            return View(prices);
        }

        public async Task<IActionResult> Create(int stockId)
        {
            var stock = await _stockService.GetByIdAsync(stockId);
            if (stock == null)
            {
                TempData["Error"] = "Stok bulunamadı.";
                return RedirectToAction("Index", "Stock");
            }

            var priceTypes = await _typeService.GetAllAsync();

            var model = new StockPriceMultiCreateDto
            {
                StockId = stock.Id,
                StockName = stock.Name,
                Prices = priceTypes.Select(pt => new StockPriceInputDto
                {
                    StockPriceTypeId = pt.Id,
                    StockPriceTypeName = pt.Name,
                    Code = pt.Code
                }).ToList()
            };

            ViewBag.Stock = stock;
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(StockPriceMultiCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Form verileri hatalı.";
                return View(model);
            }

            try
            {
                var priceList = model.Prices
                    .Where(p => p.Price > 0)
                    .Select(p => new CreateStockPriceDto
                    {
                        StockId = model.StockId,
                        StockPriceTypeId = p.StockPriceTypeId,
                        Price = p.Price
                    })
                    .ToList();

                await _service.AddOrUpdateMultipleAsync(priceList);

                TempData["Success"] = "Fiyatlar başarıyla kaydedildi.";
                return RedirectToAction("Index", "Stock", new { id = model.StockId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fiyatlar kaydedilirken hata oluştu: {ex.Message}";
                return View(model);
            }
        }


        public async Task<IActionResult> Edit(int id, int stockId)
        {
            var prices = await _service.GetByStockIdAsync(stockId);
            var item = prices.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();

            ViewBag.PriceTypes = await _typeService.GetAllAsync();
            ViewBag.Stock = await _stockService.GetByIdAsync(stockId);
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(StockPriceDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.PriceTypes = await _typeService.GetAllAsync();
                ViewBag.Stock = await _stockService.GetByIdAsync(dto.StockId);
                return View(dto);
            }

            try
            {
                await _service.UpdateAsync(dto);
                TempData["Success"] = "Fiyat başarıyla güncellendi.";
                return RedirectToAction(nameof(Index), new { stockId = dto.StockId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fiyat güncellenirken hata oluştu: {ex.Message}";
                return View(dto);
            }
        }

        public async Task<IActionResult> Delete(int id, int stockId)
        {
            var prices = await _service.GetByStockIdAsync(stockId);
            var item = prices.FirstOrDefault(x => x.Id == id);
            if (item == null) return NotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id, int stockId)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index), new { stockId });
        }
    }
}
