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
            // İş emrini detaylı çek
            var workOrder = await _unitOfWork.WorkOrders.GetDetailsAsync(workOrderId);
            if (workOrder == null)
                throw new Exception("İş emri bulunamadı.");

            // 📌 İş emri kapalı mı?
            if (workOrder.CloseDate == null)
                throw new Exception("İş emri kapatılmadan fatura kesilemez.");

            // 🔁 Zaten fatura var mı?
            var existingInvoice = await _unitOfWork.Invoices.Query()
                .FirstOrDefaultAsync(i => i.WorkOrderId == workOrder.Id);

            if (existingInvoice != null)
                throw new Exception("Bu iş emri için zaten fatura kesilmiş.");

            // 🧾 Yeni fatura oluştur
            var invoice = new Invoice
            {
                WorkOrderId = workOrder.Id,
                Date = DateTime.Now,
                LaborCost = workOrder.LaborCost, // ✅ işçilik eklendi
                Items = new List<InvoiceItem>()
            };

            decimal subTotal = 0;
            decimal totalVat = 0;

            // 🔹 Parçaları işle
            foreach (var part in workOrder.Parts)
            {
                // Fiyat bul
                var unitPrice = await _unitOfWork.StockPrices.Query()
                    .Where(sp => sp.StockId == part.StockId && sp.StockPriceTypeId == part.StockPriceTypeId)
                    .Select(sp => sp.Price)
                    .FirstOrDefaultAsync();

                var stockName = part.Stock?.Name ?? "";
                var depotName = part.Depot?.Name ?? "";
                var vatRate = part.KdvRate;

                var lineSubTotal = part.Quantity * unitPrice;
                var vatAmount = lineSubTotal * (vatRate / 100);
                var lineTotal = lineSubTotal + vatAmount;

                // Kalem ekle
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

                // Stok düş (şimdilik kontrolsüz)
                var inventory = await _unitOfWork.Inventories.Query()
                    .FirstOrDefaultAsync(i => i.StockId == part.StockId && i.DepotId == part.DepotId);

                if (inventory != null)
                {
                    inventory.Quantity -= part.Quantity;
                    _unitOfWork.Inventories.Update(inventory);
                }
            }

            // 🔸 Toplam hesapla (işçilik dahil)
            var total = subTotal + totalVat + workOrder.LaborCost;

            invoice.Total = total;

            // Kaydet
            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.CommitAsync();

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
