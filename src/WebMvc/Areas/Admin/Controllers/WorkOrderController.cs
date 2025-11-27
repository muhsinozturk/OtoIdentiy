using Application.Abstractions.Services;
using Application.DTOs.WorkOrder;
using Application.DTOs.WorkOrderPart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Areas.Admin.Controllers
{
    public class WorkOrderController : UserBaseController
    {
        private readonly IWorkOrderService _workOrderService;
        private readonly IVehicleService _vehicleService;
        private readonly IEmployeeService _employeeService;
        private readonly IStockService _stockService;
        private readonly IStockPriceService _stockPriceService;
        private readonly IInventoryService _inventoryService;
        private readonly IDepotService _depotService;
        private readonly IActService _actService;

        public WorkOrderController(
            IWorkOrderService workOrderService,
            IVehicleService vehicleService,
            IEmployeeService employeeService,
            IStockService stockService,
            IStockPriceService stockPriceService,
            IInventoryService inventoryService,
            IActService actService,
            IDepotService depotService)
        {
            _actService = actService;
            _workOrderService = workOrderService;
            _vehicleService = vehicleService;
            _employeeService = employeeService;
            _stockService = stockService;
            _stockPriceService = stockPriceService;
            _inventoryService = inventoryService;
            _depotService = depotService;
        }


        // --- Index ---
        public async Task<IActionResult> Index(int? vehicleId)
        {
            if (vehicleId.HasValue)
            {
                var vehicle = await _vehicleService.GetByIdAsync(vehicleId.Value);
                if (vehicle == null) return NotFound();

                var orders = await _workOrderService.GetByVehicleIdAsync(vehicleId.Value);
                ViewBag.Vehicle = vehicle;
                return View(orders);
            }
            else
            {
                var orders = await _workOrderService.GetAllAsync();
                ViewBag.Vehicle = null;
                return View(orders);
            }
        }


        public async Task<IActionResult> Create(int? vehicleId)
        {
            if (vehicleId.HasValue)
            {
                // Sadece seçili araca bağlı form
                var vehicle = await _vehicleService.GetByIdAsync(vehicleId.Value);
                if (vehicle == null) return NotFound();

                ViewBag.Vehicle = vehicle;
            }
            else
            {
                // Genel kullanım: müşteri ve araç seçilecek
                ViewBag.Acts = await _actService.GetAllAsync();
            }

            ViewBag.Employees = await _employeeService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateWorkOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Acts = await _actService.GetAllAsync();
                ViewBag.Employees = await _employeeService.GetAllAsync();
                return View(dto);
            }

            await _workOrderService.CreateAsync(dto);
            return RedirectToAction("Index", new { vehicleId = dto.VehicleId });
        }


        public async Task<IActionResult> Edit(int id)
        {
            var order = await _workOrderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            var updateDto = new UpdateWorkOrderDto
            {
                Id = order.Id,
                EmployeeId = order.EmployeeId,
                Description = order.Description,
                LaborCost = order.LaborCost,
                VehicleId = order.VehicleId,
            };

            ViewBag.Employees = await _employeeService.GetAllAsync();
            return View(updateDto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateWorkOrderDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = await _employeeService.GetAllAsync();
                return View(dto);
            }

            await _workOrderService.UpdateAsync(dto);
            return RedirectToAction("Index", new { vehicleId = dto.VehicleId });
        }



        // --- Delete ---
        
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _workOrderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _workOrderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            await _workOrderService.DeleteAsync(id);
            return RedirectToAction("Index", new { vehicleId = order.VehicleId });
        }


        public async Task<IActionResult> Details(int id)
        {
            var order = await _workOrderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            ViewBag.Depots = await _depotService.GetAllAsync();
            return View(order);
        }


        [HttpGet]
        public async Task<IActionResult> GetStocksByDepot(int depotId)
        {
            var inventories = await _inventoryService.GetByDepotIdAsync(depotId);

            var result = inventories
                .GroupBy(i => i.StockId)
                .Select(g => new {
                    id = g.Key,
                    name = g.First().StockName
                });

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetPriceTypesByStock(int stockId)
        {
            var prices = await _stockPriceService.GetByStockIdAsync(stockId);

            var result = prices.Select(p => new {
                id = p.StockPriceTypeId,
                name = p.StockPriceTypeName,
                price = p.Price
            });

            return Json(result);
        }

 
        [HttpPost]
        public async Task<IActionResult> AddPart(CreateWorkOrderPartDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new Exception("Geçersiz form verisi.");

                if (dto.StockPriceTypeId == null || dto.StockPriceTypeId == 0)
                    throw new Exception("Lütfen fiyat tipi seçiniz.");

                if (dto.KdvRate < 0 || dto.KdvRate > 100)
                    throw new Exception("Geçerli bir KDV oranı giriniz (0-100).");

                await _workOrderService.AddPartAsync(dto);
                TempData["Success"] = "Parça başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = dto.WorkOrderId });
        }




        [HttpPost]
        public async Task<IActionResult> RemovePart(int id, int workOrderId)
        {
            try
            {
                await _workOrderService.RemovePartAsync(id);
                TempData["Success"] = "Parça silindi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Details", new { id = workOrderId });
        }

        public async Task<IActionResult> GetStockInfo(int depotId, int stockId)
        {
            var stock = await _inventoryService.GetByDepotAndStockAsync(depotId, stockId);
            if (stock == null)
                return Json(new { exists = false, quantity = 0 });

            return Json(new { exists = true, quantity = stock.Quantity });
        }


        [HttpPost]
        public async Task<IActionResult> UpdateLaborCost(int id, decimal laborCost)
        {
            try
            {
                await _workOrderService.UpdateLaborCostAsync(id, laborCost);
                TempData["Success"] = "İşçilik başarıyla güncellendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Hata: {ex.Message}";
            }

            return RedirectToAction("Details", new { id });
        }

        // --- Ajax: Müşteriye göre araçlar ---
    
        public async Task<IActionResult> GetVehiclesByAct(int actId)
        {
            var vehicles = await _vehicleService.GetAllByActIdAsync(actId);

            var result = vehicles.Select(v => new {
                id = v.Id,
                plate = v.Plate
            });

            return Json(result);
        }


    }
}
