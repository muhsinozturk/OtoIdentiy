using Application.Abstractions.Services;
using Application.DTOs.Emplooye;
using Application.DTOs.Company;
using Microsoft.AspNetCore.Mvc;
using WebMvc.Areas.Admin.Controllers;

namespace WebMvc.Areas.Admin.Controllers
{
    public class EmployeeController : AdminBaseController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ICompanyService _companyService;

        public EmployeeController(IEmployeeService employeeService, ICompanyService companyService)
        {
            _employeeService = employeeService;
            _companyService = companyService;
        }

        // Listeleme
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllAsync();
            return View(employees);
        }

        // Detay
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // Yeni oluştur
        public async Task<IActionResult> Create()
        {
            ViewBag.Companies = await _companyService.GetAllAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync();
                return View(dto);
            }

            await _employeeService.CreateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // Düzenle
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();

            ViewBag.Companies = await _companyService.GetAllAsync();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Companies = await _companyService.GetAllAsync();
                return View(dto);
            }

            await _employeeService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // Sil
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employeeService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
