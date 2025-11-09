using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Inventory;
using Application.DTOs.Stock;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InventoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<InventoryDto?> GetByDepotAndStockAsync(int depotId, int stockId)
        {
            // 🔹 Tüm hareketleri al (giriş & çıkış)
            var movements = await _unitOfWork.Inventories.Query()
                .Where(x => x.DepotId == depotId && x.StockId == stockId && !x.IsDeleted)
                .ToListAsync();

            if (!movements.Any())
                return null;

            // 🔹 Girişler - Çıkışlar
            decimal totalInputs = movements.Where(x => x.IsInput).Sum(x => x.Quantity);
            decimal totalOutputs = movements.Where(x => !x.IsInput).Sum(x => x.Quantity);

            decimal netQuantity = totalInputs - totalOutputs;

            // 🔹 İlk kayıttan temel bilgileri al
            var sample = movements.First();

            return new InventoryDto
            {
                StockId = stockId,
                DepotId = depotId,
                Quantity = netQuantity,
                StockName = sample.Stock.Name,
                DepotName = sample.Stock.Name
            };
        }

        public async Task<List<InventoryDto>> GetByFilterAsync(int depotId, DateTime? startDate, DateTime? endDate, bool? isInput, string? search)
        {
            var query = _unitOfWork.Inventories.Query()
                .Include(x => x.Stock)
                .Include(x => x.Depot)
                .Where(x => x.DepotId == depotId && !x.IsDeleted);

            if (isInput.HasValue)
                query = query.Where(x => x.IsInput == isInput.Value);

            if (startDate.HasValue)
                query = query.Where(x => x.CreatedAt.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(x => x.CreatedAt.Date <= endDate.Value.Date);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(x =>
                    x.Stock.Name.ToLower().Contains(search) ||
                    x.Stock.Code.ToLower().Contains(search) ||
                    x.Stock.Brand.ToLower().Contains(search) ||
                    x.Stock.Model.ToLower().Contains(search));
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

        public async Task<decimal> GetCurrentStockAsync(int stockId, int depotId)
        {
            var entries = await _unitOfWork.Inventories.Query()
                .Where(x => x.StockId == stockId && x.DepotId == depotId)
                .ToListAsync();

            var totalIn = entries.Where(x => x.IsInput).Sum(x => x.Quantity);
            var totalOut = entries.Where(x => !x.IsInput).Sum(x => x.Quantity);

            return totalIn - totalOut;
        }

        public async Task<List<InventoryDto>> GetByDepotIdAsync(int depotId)
        {
            var list = await _unitOfWork.Inventories.Query()
                .Include(x => x.Stock)
                .ThenInclude(s => s.StockGroup)
                .Include(x => x.Depot)
                .Where(x => x.DepotId == depotId && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<InventoryDto>>(list);
        }


        public async Task CreateAsync(CreateInventoryDto dto)
        {
            var entity = new Inventory
            {
                DepotId = dto.DepotId,
                StockId = dto.StockId,
                Quantity = dto.Quantity,
                IsInput = dto.IsInput, // ✅ DTO’dan gelen true
                Description = dto.Description ?? "Manuel stok girişi",
                CreatedAt = DateTimeOffset.Now
            };

            await _unitOfWork.Inventories.AddAsync(entity);
            await _unitOfWork.CommitAsync();
        }


        public async Task UpdateAsync(InventoryDto dto)
        {
            var entity = await _unitOfWork.Inventories.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new Exception("Inventory not found");

            _mapper.Map(dto, entity);
            _unitOfWork.Inventories.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Inventories.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Inventory not found");

            _unitOfWork.Inventories.Remove(entity);
            await _unitOfWork.CommitAsync();
        }
        public async Task<List<StockDto>> GetByGroupIdAsync(int groupId)
        {
            var stocks = await _unitOfWork.Stocks.GetAllAsync();
            var filtered = stocks.Where(s => s.StockGroupId == groupId).ToList();
            return _mapper.Map<List<StockDto>>(filtered);
        }

        public async Task<List<DepotInventorySummaryDto>> GetDepotSummaryAsync(int depotId)
        {
            var inventories = await _unitOfWork.Inventories
                .Query()
                .Include(x => x.Stock)
                .ThenInclude(s => s.StockGroup)
                .Where(x => x.DepotId == depotId && !x.IsDeleted)
                .ToListAsync();

            var result = inventories
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

                    // 🔹 Giriş ve çıkış miktarlarını ayrı hesapla
                    TotalIn = g.Where(i => i.IsInput).Sum(i => i.Quantity),
                    TotalOut = g.Where(i => !i.IsInput).Sum(i => i.Quantity),

                    // 🔹 Detay item listesi (istersen view'de tablo altı detay için)
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

            return result;
        }



    }
}
