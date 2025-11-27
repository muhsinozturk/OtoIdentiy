using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Inventory;
using Application.DTOs.Stock;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _http = http;
        }

        private async Task<Depot> GetOwnedDepotAsync(int depotId)
        {
            var depot = await _unitOfWork.Depots
                .Query()
                .Include(d => d.Company)
                .FirstOrDefaultAsync(d => d.Id == depotId);

            if (depot == null)
                throw new Exception("Depo bulunamadı.");

            var userId = _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier);

            if (depot.Company.CreatedUserId != userId)
                throw new UnauthorizedAccessException("Bu depoya erişim yetkiniz yok.");

            return depot;
        }

        // 🔹 Depodaki stok miktarı + güvenlik
        public async Task<InventoryDto?> GetByDepotAndStockAsync(int depotId, int stockId)
        {
            await GetOwnedDepotAsync(depotId);

            var movements = await _unitOfWork.Inventories.Query()
                .Include(x => x.Stock)
                .Include(x => x.Depot)
                .Where(x => x.DepotId == depotId && x.StockId == stockId)
                .ToListAsync();

            if (!movements.Any())
                return null;

            decimal totalInputs = movements.Where(x => x.IsInput).Sum(x => x.Quantity);
            decimal totalOutputs = movements.Where(x => !x.IsInput).Sum(x => x.Quantity);

            var sample = movements.First();

            return new InventoryDto
            {
                StockId = stockId,
                DepotId = depotId,
                Quantity = totalInputs - totalOutputs,
                StockName = sample.Stock.Name,
                DepotName = sample.Depot.Name
            };
        }

        // 🔹 Hareket listesi
        public async Task<List<InventoryDto>> GetByFilterAsync(int depotId, DateTime? start, DateTime? end, bool? isInput, string? search)
        {
            await GetOwnedDepotAsync(depotId);

            var query = _unitOfWork.Inventories.Query()
                .Include(x => x.Stock)
                .Include(x => x.Depot)
                .Where(x => x.DepotId == depotId);

            if (isInput.HasValue)
                query = query.Where(x => x.IsInput == isInput.Value);

            if (start.HasValue)
                query = query.Where(x => x.CreatedAt.Date >= start.Value.Date);

            if (end.HasValue)
                query = query.Where(x => x.CreatedAt.Date <= end.Value.Date);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    x.Stock.Name.ToLower().Contains(search) ||
                    x.Stock.Code.ToLower().Contains(search) ||
                    x.Stock.Brand.ToLower().Contains(search) ||
                    x.Stock.Model.ToLower().Contains(search)
                );
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new InventoryDto
                {
                    Id = x.Id,
                    DepotId = x.DepotId,
                    DepotName = x.Depot.Name,
                    StockId = x.StockId,
                    StockName = x.Stock.Name,
                    Quantity = x.Quantity,
                    IsInput = x.IsInput,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        // 🔹 Depodaki anlık stok
        public async Task<decimal> GetCurrentStockAsync(int stockId, int depotId)
        {
            await GetOwnedDepotAsync(depotId);

            var entries = await _unitOfWork.Inventories.Query()
                .Where(x => x.StockId == stockId && x.DepotId == depotId)
                .ToListAsync();

            return entries.Where(x => x.IsInput).Sum(x => x.Quantity)
                 - entries.Where(x => !x.IsInput).Sum(x => x.Quantity);
        }

        // 🔹 Depoya ait tüm hareketler
        public async Task<List<InventoryDto>> GetByDepotIdAsync(int depotId)
        {
            await GetOwnedDepotAsync(depotId);

            var list = await _unitOfWork.Inventories.Query()
                .Include(x => x.Stock).ThenInclude(s => s.StockGroup)
                .Include(x => x.Depot)
                .Where(x => x.DepotId == depotId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<InventoryDto>>(list);
        }

        // 🔹 Yeni hareket
        public async Task CreateAsync(CreateInventoryDto dto)
        {
            await GetOwnedDepotAsync(dto.DepotId);

            var userId = _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Kullanıcı bulunamadı.");

            var entity = new Inventory
            {
                DepotId = dto.DepotId,
                StockId = dto.StockId,
                Quantity = dto.Quantity,
                IsInput = dto.IsInput,
                Description = dto.Description ?? "",
                CreatedAt = DateTimeOffset.Now,
                CreatedUserId = userId   // 🔥 ZORUNLU
            };

            await _unitOfWork.Inventories.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }


        // 🔹 Güncelleme
        public async Task UpdateAsync(InventoryDto dto)
        {
            var entity = await _unitOfWork.Inventories.Query()
                .Include(x => x.Depot).ThenInclude(c => c.Company)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Stok hareketi bulunamadı.");

            await GetOwnedDepotAsync(entity.DepotId);

            _mapper.Map(dto, entity);

            _unitOfWork.Inventories.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Silme
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Inventories.Query()
                .Include(x => x.Depot).ThenInclude(c => c.Company)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Hareket bulunamadı.");

            await GetOwnedDepotAsync(entity.DepotId);

            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.Now;

            _unitOfWork.Inventories.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Stok grubu filtre
        public async Task<List<StockDto>> GetByGroupIdAsync(int groupId)
        {
            var stocks = await _unitOfWork.Stocks.GetAllAsync();
            return _mapper.Map<List<StockDto>>(stocks.Where(s => s.StockGroupId == groupId));
        }

        // 🔹 Depo özet
        public async Task<List<DepotInventorySummaryDto>> GetDepotSummaryAsync(int depotId)
        {
            await GetOwnedDepotAsync(depotId);

            var inventories = await _unitOfWork.Inventories.Query()
                .Include(x => x.Stock).ThenInclude(s => s.StockGroup)
                .Where(x => x.DepotId == depotId)
                .ToListAsync();

            return inventories
               .GroupBy(x => new
               {
                   StockGroupName = x.Stock.StockGroup.Name,
                   StockName = x.Stock.Name,
                   StockModel = x.Stock.Model,
                   StockBrand = x.Stock.Brand
               })

                .Select(g => new DepotInventorySummaryDto
                {
                    StockGroupName = g.Key.StockGroupName,
                    StockName = g.Key.StockName,
                    StockModel = g.Key.StockModel,
                    StockBrand = g.Key.StockBrand,
                    TotalIn = g.Where(i => i.IsInput).Sum(i => i.Quantity),
                    TotalOut = g.Where(i => !i.IsInput).Sum(i => i.Quantity),
                    Items = g.Select(i => new DepotInventoryItemDto
                    {
                        StockId = i.StockId,
                        StockName = i.Stock.Name,
                        Quantity = i.Quantity,
                        IsInput = i.IsInput,
                        CreatedAt = i.CreatedAt
                    }).ToList()
                })
                .OrderBy(r => r.StockGroupName)
                .ThenBy(r => r.StockName)
                .ToList();
        }
    }
}
