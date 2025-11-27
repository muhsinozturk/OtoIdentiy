using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Invoice;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _http = http;
        }

        private async Task<WorkOrder> GetOwnedWorkOrderAsync(int workOrderId)
        {
            var wo = await _unitOfWork.WorkOrders
                .Query()
                .Include(w => w.Vehicle)
                .ThenInclude(v => v.Act)
                .ThenInclude(a => a.Company)
                .FirstOrDefaultAsync(w => w.Id == workOrderId);

            if (wo == null)
                throw new Exception("İş emri bulunamadı.");

            var userId = _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier);

            if (wo.Vehicle.Act.Company.CreatedUserId != userId)
                throw new UnauthorizedAccessException("Bu iş emri size ait değildir.");

            return wo;
        }

        // 🔹 Fatura oluşturma
        public async Task<InvoiceDto> CreateFromWorkOrderAsync(int workOrderId)
        {
            var workOrder = await GetOwnedWorkOrderAsync(workOrderId);

            if (workOrder.CloseDate == null)
                throw new Exception("İş emri kapatılmadan fatura kesilemez.");

            var existingInvoice = await _unitOfWork.Invoices
                .Query()
                .FirstOrDefaultAsync(i => i.WorkOrderId == workOrderId);

            if (existingInvoice != null)
                throw new Exception("Bu iş emri için zaten fatura kesilmiş.");

            var invoice = new Invoice
            {
                WorkOrderId = workOrder.Id,
                Date = DateTime.Now,
                LaborCost = workOrder.LaborCost,
                Items = new List<InvoiceItem>()
            };

            decimal subTotal = 0;
            decimal totalVat = 0;

            foreach (var part in workOrder.Parts)
            {
                var unitPrice = await _unitOfWork.StockPrices
                    .Query()
                    .Where(sp => sp.StockId == part.StockId && sp.StockPriceTypeId == part.StockPriceTypeId)
                    .Select(sp => sp.Price)
                    .FirstOrDefaultAsync();

                if (unitPrice <= 0)
                    throw new Exception("Stok fiyatı bulunamadı.");

                var vatAmount = (part.Quantity * unitPrice) * (part.KdvRate / 100);

                invoice.Items.Add(new InvoiceItem
                {
                    StockId = part.StockId,
                    StockName = part.Stock?.Name,
                    DepotId = part.DepotId,
                    DepotName = part.Depot?.Name,
                    Quantity = part.Quantity,
                    UnitPrice = unitPrice,
                    KdvRate = part.KdvRate
                });

                subTotal += part.Quantity * unitPrice;
                totalVat += vatAmount;
            }

            invoice.Total = subTotal + totalVat + workOrder.LaborCost;

            // 🔥 Önce faturayı kaydet
            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.CommitAsync();

            // 🔥 Sonra stoktan düş
            foreach (var part in workOrder.Parts)
            {
                var movement = new Inventory
                {
                    DepotId = part.DepotId,
                    StockId = part.StockId,
                    Quantity = part.Quantity,
                    IsInput = false,
                    Description = $"Fatura #{invoice.Id} çıkışı",
                    CreatedAt = DateTimeOffset.Now
                };

                await _unitOfWork.Inventories.AddAsync(movement);
            }

            await _unitOfWork.CommitAsync();

            return _mapper.Map<InvoiceDto>(invoice);
        }

        // 🔹 Kullanıcının tüm faturaları
        public async Task<List<InvoiceDto>> GetAllAsync()
        {
            var userId = _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier);

            var invoices = await _unitOfWork.Invoices
                .Query()
                .Include(i => i.WorkOrder)
                .ThenInclude(w => w.Vehicle)
                .ThenInclude(v => v.Act)
                .ThenInclude(a => a.Company)
                .Where(i => i.WorkOrder.Vehicle.Act.Company.CreatedUserId == userId)
                .ToListAsync();

            return _mapper.Map<List<InvoiceDto>>(invoices);
        }

        // 🔹 Tek fatura (yetki kontrollü)
        public async Task<InvoiceDto?> GetByIdAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices
                .Query()
                .Include(i => i.WorkOrder)
                .ThenInclude(w => w.Vehicle)
                .ThenInclude(v => v.Act)
                .ThenInclude(a => a.Company)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice == null)
                return null;

            var userId = _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier);

            if (invoice.WorkOrder.Vehicle.Act.Company.CreatedUserId != userId)
                throw new UnauthorizedAccessException("Bu faturayı görme yetkiniz yok.");

            return _mapper.Map<InvoiceDto>(invoice);
        }
    }
}
