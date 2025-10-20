using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.WorkOrder;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WorkOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // --- Araç bazlı iş emirleri ---
        public async Task<List<WorkOrderDto>> GetByVehicleIdAsync(int vehicleId)
        {
            var workOrders = await _unitOfWork.WorkOrders.GetAllWithDetailsAsync();
            var filtered = workOrders.Where(w => w.VehicleId == vehicleId).ToList();
            return _mapper.Map<List<WorkOrderDto>>(filtered);
        }

        // --- Tüm iş emirleri ---
        public async Task<List<WorkOrderDto>> GetAllAsync()
        {
            var orders = await _unitOfWork.WorkOrders.GetAllWithDetailsAsync();
            var dtos = _mapper.Map<List<WorkOrderDto>>(orders);

            foreach (var dto in dtos)
            {
                var invoice = await _unitOfWork.Invoices.Query()
                    .FirstOrDefaultAsync(i => i.WorkOrderId == dto.Id);

                if (invoice != null)
                {
                    dto.HasInvoice = true;
                    dto.InvoiceId = invoice.Id;
                }
            }

            return dtos;
        }



        // --- Tek iş emri (detaylı) ---
        public async Task<WorkOrderDto?> GetByIdAsync(int id)
        {
            var workOrder = await _unitOfWork.WorkOrders.GetDetailsAsync(id);
            if (workOrder == null) return null;

            var dto = _mapper.Map<WorkOrderDto>(workOrder);

            // Her part için fiyatı StockPrices tablosundan doldur
            foreach (var part in dto.Parts)
            {
                var price = await _unitOfWork.StockPrices.Query()
                    .Where(sp => sp.StockId == part.StockId &&
                                 sp.StockPriceTypeId == part.StockPriceTypeId)
                    .Select(sp => sp.Price)
                    .FirstOrDefaultAsync();

                part.StockPrice = price;
            }

            // 🔽 Fatura kontrolü
            var invoice = await _unitOfWork.Invoices.Query()
                .FirstOrDefaultAsync(i => i.WorkOrderId == dto.Id);

            if (invoice != null)
                dto.InvoiceId = invoice.Id;

            return dto;
        }



        // --- İş emri açma ---
        public async Task<WorkOrderDto> CreateAsync(CreateWorkOrderDto dto)
        {
            var entity = _mapper.Map<WorkOrder>(dto);
            entity.OpenDate = DateTime.Now; // açılış tarihi
            await _unitOfWork.WorkOrders.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<WorkOrderDto>(entity);
        }

        // --- İş emri kapatma / güncelleme ---
        public async Task CloseWorkOrderAsync(int id, string description, decimal laborCost, int? employeeId)
        {
            var workOrder = await _unitOfWork.WorkOrders.GetDetailsAsync(id);
            if (workOrder == null)
                throw new Exception("WorkOrder not found");

            workOrder.Description = description;
            workOrder.LaborCost = laborCost;
            workOrder.EmployeeId = employeeId;
            workOrder.CloseDate = DateTime.Now;

            // Eğer iş emrine bağlı fatura yoksa oluştur
            var existingInvoice = await _unitOfWork.Invoices.Query()
                .FirstOrDefaultAsync(i => i.WorkOrderId == workOrder.Id);

            if (existingInvoice == null)
            {
                var invoice = new Invoice
                {
                    WorkOrderId = workOrder.Id,
                    Date = DateTime.Now,
                    Items = new List<InvoiceItem>()
                };

                decimal total = 0;

                // Parça kalemleri
                foreach (var part in workOrder.Parts)
                {
                    var unitPrice = await _unitOfWork.StockPrices.Query()
                        .Where(sp => sp.StockId == part.StockId && sp.StockPriceTypeId == part.StockPriceTypeId)
                        .Select(sp => sp.Price)
                        .FirstOrDefaultAsync();

                    invoice.Items.Add(new InvoiceItem
                    {
                        StockId = part.StockId,
                        StockName = part.Stock.Name,
                        DepotId = part.DepotId,
                        DepotName = part.Depot.Name,
                        Quantity = part.Quantity,
                        UnitPrice = unitPrice
                    });

                    total += part.Quantity * unitPrice;

                    // Stok düş
                    var inventory = await _unitOfWork.Inventories.Query()
                        .FirstOrDefaultAsync(i => i.StockId == part.StockId && i.DepotId == part.DepotId);

                    if (inventory != null)
                    {
                        inventory.Quantity -= part.Quantity;
                        _unitOfWork.Inventories.Update(inventory);
                    }
                }

                // İşçilik ekle
                total += workOrder.LaborCost;
                invoice.Total = total;

                await _unitOfWork.Invoices.AddAsync(invoice);
            }

            await _unitOfWork.CommitAsync();
        }


        // --- İş emri silme ---
        public async Task DeleteAsync(int id)
        {
            var workOrder = await _unitOfWork.WorkOrders.GetByIdAsync(id);
            if (workOrder == null)
                throw new Exception("WorkOrder not found");

            _unitOfWork.WorkOrders.Remove(workOrder);
            await _unitOfWork.CommitAsync();
        }

        // --- İş emrine parça ekleme ---
        public async Task AddPartAsync(CreateWorkOrderPartDto dto)
        {
            var entity = _mapper.Map<WorkOrderPart>(dto);
            await _unitOfWork.WorkOrderParts.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        // --- İş emrinden parça silme ---
        public async Task RemovePartAsync(int partId)
        {
            var part = await _unitOfWork.WorkOrderParts.GetByIdAsync(partId);
            if (part == null)
                throw new Exception("WorkOrderPart not found");

            _unitOfWork.WorkOrderParts.Remove(part);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(UpdateWorkOrderDto dto)
        {
            var workOrder = await _unitOfWork.WorkOrders.GetByIdAsync(dto.Id);
            if (workOrder == null)
                throw new Exception("WorkOrder not found");

            workOrder.Description = dto.Description;
            workOrder.LaborCost = dto.LaborCost;
            workOrder.EmployeeId = dto.EmployeeId;
            workOrder.CloseDate = DateTime.Now;

            _unitOfWork.WorkOrders.Update(workOrder);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateLaborCostAsync(int id, decimal laborCost)
        {
            var workOrder = await _unitOfWork.WorkOrders.GetByIdAsync(id);
            if (workOrder == null)
                throw new Exception("İş emri bulunamadı");

            workOrder.LaborCost = laborCost;
            _unitOfWork.WorkOrders.Update(workOrder);
            await _unitOfWork.CommitAsync();
        }


    }
}
