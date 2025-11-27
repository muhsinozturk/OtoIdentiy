using Application.Abstractions.Services;
using Microsoft.AspNetCore.Mvc;
namespace WebMvc.Areas.User.Controllers;

public class InvoiceController : UserBaseController
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var invoices = await _invoiceService.GetAllAsync();
        return View(invoices);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var invoice = await _invoiceService.GetByIdAsync(id);
        if (invoice == null) return NotFound();

        return View(invoice);
    }

  [HttpPost]
public async Task<IActionResult> CreateFromWorkOrder(int workOrderId, int? vehicleId)
{
    await _invoiceService.CreateFromWorkOrderAsync(workOrderId);

    if (vehicleId.HasValue)
        return RedirectToAction("Index", "WorkOrder", new { vehicleId = vehicleId.Value });

    return RedirectToAction("Index", "WorkOrder");
}

}
