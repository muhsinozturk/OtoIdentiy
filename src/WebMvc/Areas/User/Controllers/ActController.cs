using Application.Abstractions.Services;
using Application.DTOs.Act;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Areas.User.Controllers
{

    public class ActController : UserBaseController
    {
        private readonly IActService _actService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<AppUser> _userManager;
        public ActController(IActService actService, ICompanyService companyService, UserManager<AppUser> userManager)
        {
            _actService = actService;
            _companyService = companyService;
            _userManager = userManager;
        }

        // Müşteri listesi
        public async Task<IActionResult> Index()
        {
            var acts = await _actService.GetAllAsync();
          
            return View(acts);
        }

        // Yeni müşteri ekleme (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Companies = await _companyService.GetAllAsync();
            return View(new CreateActDto());
        }

        // Yeni müşteri ekleme (POST)
        [HttpPost]
        public async Task<IActionResult> Create(CreateActDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync();
                return View(dto);
            }

            await _actService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // Müşteri düzenleme (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var act = await _actService.GetByIdAsync(id);
            if (act == null) return NotFound();

            ViewBag.Companies = await _companyService.GetAllAsync();
            return View(act);
        }

        // Müşteri düzenleme (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(ActDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync();
                return View(dto);
            }

            await _actService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // Müşteri silme (GET)
   
        public async Task<IActionResult> Delete(int id)
        {
            var act = await _actService.GetByIdAsync(id);
            if (act == null) return NotFound();

            return View(act);
        }

        // Müşteri silme (POST)
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _actService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Müşteri detayları (AYRI SAYFA)
        public async Task<IActionResult> Details(int id)
        {
            var act = await _actService.GetWithVehiclesAsync(id);
            if (act == null) return NotFound();

            return View(act);
        }

        // Müşteri detayları (INDEX İÇİN PARTIAL)
        [HttpGet]
        public async Task<IActionResult> DetailsPartial(int id)
        {
            var act = await _actService.GetWithVehiclesAsync(id);
            if (act == null) return NotFound();

            return PartialView("_ActDetailsPartial", act);
        }
    }
}
