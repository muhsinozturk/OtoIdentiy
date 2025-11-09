using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Invoice;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InvoiceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<InvoiceDto> CreateFromWorkOrderAsync(int workOrderId)
        {
            // 🔹 İş emrini al
            var workOrder = await _unitOfWork.WorkOrders.GetDetailsAsync(workOrderId);
            if (workOrder == null)
                throw new Exception("İş emri bulunamadı.");

            if (workOrder.CloseDate == null)
                throw new Exception("İş emri kapatılmadan fatura kesilemez.");

            var existingInvoice = await _unitOfWork.Invoices.Query()
                .FirstOrDefaultAsync(i => i.WorkOrderId == workOrder.Id);

            if (existingInvoice != null)
                throw new Exception("Bu iş emri için zaten fatura kesilmiş.");

            // 🧾 Yeni fatura
            var invoice = new Invoice
            {
                WorkOrderId = workOrder.Id,
                Date = DateTime.Now,
                LaborCost = workOrder.LaborCost,
                Items = new List<InvoiceItem>()
            };

            decimal subTotal = 0;
            decimal totalVat = 0;

            // 🔹 Faturadaki kalemleri oluştur
            foreach (var part in workOrder.Parts)
            {
                var unitPrice = await _unitOfWork.StockPrices.Query()
                    .Where(sp => sp.StockId == part.StockId && sp.StockPriceTypeId == part.StockPriceTypeId)
                    .Select(sp => sp.Price)
                    .FirstOrDefaultAsync();

                var stockName = part.Stock?.Name ?? "";
                var depotName = part.Depot?.Name ?? "";
                var vatRate = part.KdvRate;

                var lineSubTotal = part.Quantity * unitPrice;
                var vatAmount = lineSubTotal * (vatRate / 100);

                invoice.Items.Add(new InvoiceItem
                {
                    StockId = part.StockId,
                    StockName = stockName,
                    DepotId = part.DepotId,
                    DepotName = depotName,
                    Quantity = part.Quantity,
                    UnitPrice = unitPrice,
                    KdvRate = vatRate
                });

                subTotal += lineSubTotal;
                totalVat += vatAmount;
            }

            // 🔹 Toplam hesapla
            invoice.Total = subTotal + totalVat + workOrder.LaborCost;

            // 🧾 Önce faturayı kaydet → ID al
            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.CommitAsync(); // ✅ artık invoice.Id doldu

            // 🔹 Şimdi stoktan düş ve hareket oluştur
            foreach (var part in workOrder.Parts)
            {
                // stoktan düş
                var inventory = await _unitOfWork.Inventories.Query()
                    .FirstOrDefaultAsync(i => i.StockId == part.StockId && i.DepotId == part.DepotId);

           

                // çıkış hareketi oluştur
                var movement = new Inventory
                {
                    DepotId = part.DepotId,
                    StockId = part.StockId,
                    Quantity = part.Quantity,
                    IsInput = false,
                    CreatedAt = DateTime.Now,
                    Description = $"Fatura #{invoice.Id} çıkışı (İş Emri #{workOrder.Id})"
                };

                await _unitOfWork.Inventories.AddAsync(movement);
            }

            await _unitOfWork.CommitAsync(); // ✅ ikinci commit → çıkışlar kaydedilir

            return _mapper.Map<InvoiceDto>(invoice);
        }

        public async Task<List<InvoiceDto>> GetAllAsync()
        {
            var invoices = await _unitOfWork.Invoices.GetAllAsync();
            return _mapper.Map<List<InvoiceDto>>(invoices);
        }

        public async Task<InvoiceDto?> GetByIdAsync(int id)
        {
            var invoice = await _unitOfWork.Invoices.GetByIdAsync(id);
            return invoice == null ? null : _mapper.Map<InvoiceDto>(invoice);
        }
    }
}
