using Application.Abstractions.Services;
using Application.DTOs.StockGroup;
using Microsoft.AspNetCore.Mvc;

namespace WebMvc.Areas.User.Controllers;



public class StockGroupController : UserBaseController
{
    private readonly IStockGroupService _stockGroupService;

    public StockGroupController(IStockGroupService stockGroupService)
    {
        _stockGroupService = stockGroupService;
    }

    public async Task<IActionResult> Index()
    {
        var groups = await _stockGroupService.GetAllAsync();
        return View(groups);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateStockGroupDto());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateStockGroupDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _stockGroupService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var group = await _stockGroupService.GetByIdAsync(id);
        if (group == null) return NotFound();

        var dto = new StockGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            UnitType = group.UnitType
        };

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(StockGroupDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _stockGroupService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var group = await _stockGroupService.GetByIdAsync(id);
        if (group == null) return NotFound();

        return View(group);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _stockGroupService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
