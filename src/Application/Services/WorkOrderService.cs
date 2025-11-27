using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.WorkOrder;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services
{
    public class WorkOrderService : IWorkOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public WorkOrderService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _http = http;
        }

        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // -----------------------------------------
        // USERQUERY TABANLI TEK GÜVENLİK NOKTASI
        // -----------------------------------------
        private IQueryable<WorkOrder> BaseQuery =>
        _unitOfWork.WorkOrders.Query()
        .Include(w => w.Vehicle)
            .ThenInclude(v => v.Act)
            .ThenInclude(a => a.Company)
        .Include(w => w.Parts)
            .ThenInclude(p => p.Stock)
        .Include(w => w.Parts)
            .ThenInclude(p => p.Depot)
        .Where(w => w.Vehicle.Act.Company.CreatedUserId == UserId);



        private async Task<WorkOrder> GetOwnedAsync(int id)
        {
            var wo = await BaseQuery.FirstOrDefaultAsync(x => x.Id == id);

            if (wo == null)
                throw new Exception("Bu iş emri size ait değil veya bulunamadı.");

            return wo;
        }

        // -----------------------------------------
        // GET ALL
        // -----------------------------------------
        public async Task<List<WorkOrderDto>> GetAllAsync()
        {
            var list = await BaseQuery.ToListAsync();
            var dtos = _mapper.Map<List<WorkOrderDto>>(list);

            foreach (var dto in dtos)
            {
                dto.InvoiceId = await _unitOfWork.Invoices
                    .UserQuery(UserId)
                    .Where(i => i.WorkOrderId == dto.Id)
                    .Select(i => (int?)i.Id)
                    .FirstOrDefaultAsync();

                dto.HasInvoice = dto.InvoiceId != null;
            }

            return dtos;
        }

        // -----------------------------------------
        // GET BY VEHICLE
        // -----------------------------------------
        public async Task<List<WorkOrderDto>> GetByVehicleIdAsync(int vehicleId)
        {
            var list = await BaseQuery
                .Where(w => w.VehicleId == vehicleId)
                .ToListAsync();

            return _mapper.Map<List<WorkOrderDto>>(list);
        }

        // -----------------------------------------
        // DETAILS
        // -----------------------------------------
        public async Task<WorkOrderDto?> GetByIdAsync(int id)
        {
            var wo = await GetOwnedAsync(id);
            var dto = _mapper.Map<WorkOrderDto>(wo);

            foreach (var part in dto.Parts)
            {
                part.StockPrice = await _unitOfWork.StockPrices.UserQuery(UserId)
                    .Where(sp => sp.StockId == part.StockId &&
                                 sp.StockPriceTypeId == part.StockPriceTypeId)
                    .Select(sp => sp.Price)
                    .FirstOrDefaultAsync();
            }

            var invoice = await _unitOfWork.Invoices.UserQuery(UserId)
                .FirstOrDefaultAsync(i => i.WorkOrderId == id);

            dto.InvoiceId = invoice?.Id;
            dto.HasInvoice = invoice != null;

            return dto;
        }

        // -----------------------------------------
        // CREATE
        // -----------------------------------------
        public async Task<WorkOrderDto> CreateAsync(CreateWorkOrderDto dto)
        {
            var entity = _mapper.Map<WorkOrder>(dto);
            entity.CreatedUserId = UserId;
            entity.OpenDate = DateTime.Now;

            await _unitOfWork.WorkOrders.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<WorkOrderDto>(entity);
        }

        // -----------------------------------------
        // UPDATE (JOB CLOSE DEĞİL)
        // -----------------------------------------
        public async Task UpdateAsync(UpdateWorkOrderDto dto)
        {
            var wo = await GetOwnedAsync(dto.Id);

            wo.Description = dto.Description;
            wo.LaborCost = dto.LaborCost;
            wo.EmployeeId = dto.EmployeeId;

            _unitOfWork.WorkOrders.Update(wo);
            await _unitOfWork.CommitAsync();
        }

        // -----------------------------------------
        // UPDATE LABOR
        // -----------------------------------------
        public async Task UpdateLaborCostAsync(int id, decimal laborCost)
        {
            var wo = await GetOwnedAsync(id);
            wo.LaborCost = laborCost;

            await _unitOfWork.CommitAsync();
        }

        // -----------------------------------------
        // CLOSE WORK ORDER → AUTO INVOICE
        // -----------------------------------------
        public async Task CloseWorkOrderAsync(int id, string description, decimal laborCost, int? employeeId)
        {
            var wo = await GetOwnedAsync(id);

            wo.Description = description;
            wo.LaborCost = laborCost;
            wo.EmployeeId = employeeId;
            wo.CloseDate = DateTime.Now;

            var existingInvoice = await _unitOfWork.Invoices
                .UserQuery(UserId)
                .FirstOrDefaultAsync(i => i.WorkOrderId == id);

            if (existingInvoice == null)
            {
                var invoice = new Invoice
                {
                    WorkOrderId = id,
                    Date = DateTime.Now,
                    CreatedUserId = UserId,
                    Items = new List<InvoiceItem>()
                };

                await _unitOfWork.Invoices.AddAsync(invoice);
                await _unitOfWork.CommitAsync();

                decimal total = 0;

                foreach (var part in wo.Parts)
                {
                    var unitPrice = await _unitOfWork.StockPrices.UserQuery(UserId)
                        .Where(sp => sp.StockId == part.StockId &&
                                     sp.StockPriceTypeId == part.StockPriceTypeId)
                        .Select(sp => sp.Price)
                        .FirstOrDefaultAsync();

                    invoice.Items.Add(new InvoiceItem
                    {
                        InvoiceId = invoice.Id,
                        StockId = part.StockId,
                        StockName = part.Stock.Name,
                        DepotId = part.DepotId,
                        DepotName = part.Depot.Name,
                        Quantity = part.Quantity,
                        UnitPrice = unitPrice,
                        CreatedUserId = UserId,
                        CreatedAt = DateTime.Now
                    });

                    total += part.Quantity * unitPrice;

                    await _unitOfWork.Inventories.AddAsync(new Inventory
                    {
                        DepotId = part.DepotId,
                        StockId = part.StockId,
                        Quantity = part.Quantity,
                        IsInput = false,
                        Description = $"Fatura #{invoice.Id} çıkışı",
                        CreatedAt = DateTime.Now,
                        CreatedUserId = UserId
                    });
                }

                invoice.Total = total + laborCost;
            }

            await _unitOfWork.CommitAsync();
        }

        // -----------------------------------------
        // DELETE
        // -----------------------------------------
        public async Task DeleteAsync(int id)
        {
            var wo = await GetOwnedAsync(id);
            _unitOfWork.WorkOrders.Remove(wo);
            await _unitOfWork.CommitAsync();
        }

        // -----------------------------------------
        // ADD PART
        // -----------------------------------------
        public async Task AddPartAsync(CreateWorkOrderPartDto dto)
        {
            var wo = await GetOwnedAsync(dto.WorkOrderId);

            var list = await _unitOfWork.Inventories.UserQuery(UserId)
                .Where(i => i.DepotId == dto.DepotId && i.StockId == dto.StockId)
                .ToListAsync();

            var stock = list.Where(x => x.IsInput).Sum(x => x.Quantity) -
                        list.Where(x => !x.IsInput).Sum(x => x.Quantity);

            if (stock < dto.Quantity)
                throw new Exception("Yeterli stok yok.");

            var entity = _mapper.Map<WorkOrderPart>(dto);
            entity.CreatedUserId = UserId;
            entity.CreatedAt = DateTime.Now;

            await _unitOfWork.WorkOrderParts.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }

        // -----------------------------------------
        // REMOVE PART
        // -----------------------------------------
        public async Task RemovePartAsync(int partId)
        {
            var part = await _unitOfWork.WorkOrderParts.UserQuery(UserId)
                .FirstOrDefaultAsync(p => p.Id == partId);

            if (part == null)
                throw new Exception("Bu parça size ait değil.");

            _unitOfWork.WorkOrderParts.Remove(part);
            await _unitOfWork.CommitAsync();
        }
    }
}
