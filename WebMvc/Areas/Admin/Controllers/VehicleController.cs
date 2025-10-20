using Application.Abstractions.Services;
using Application.DTOs.Vehicle;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Areas.Admin.Controllers
{

    public class VehicleController : AdminBaseController
    {
        private readonly IVehicleService _vehicleService;
        private readonly IActService _actService;

        public VehicleController(IVehicleService vehicleService, IActService actService)
        {
            _vehicleService = vehicleService;
            _actService = actService;
        }
        public async Task<IActionResult> Index(int? actId)
        {
            var vehicles = actId.HasValue && actId > 0
                ? await _vehicleService.GetAllByActIdAsync(actId.Value)
                : await _vehicleService.GetAllAsync();

            // Müşteri bilgisi sadece actId varsa alınsın
            if (actId.HasValue && actId > 0)
            {
                var act = await _actService.GetByIdAsync(actId.Value);
                ViewBag.ActName = act?.FullName;
                ViewBag.ActId = actId;
            }
            else
            {
                ViewBag.ActName = "Tüm Araçlar";
                ViewBag.ActId = null;
            }

            return View(vehicles);
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
            return RedirectToAction("Index", "Act", new { area = "Admin", id = dto.ActId });
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
            return RedirectToAction("Index", "Act", new { area = "Admin", id = dto.ActId });
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
            return RedirectToAction("Index", "Act", new { area = "Admin", id = vehicle.ActId });
        }
    }
}
