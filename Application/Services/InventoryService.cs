using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Inventory;
using Application.DTOs.Stock;
using AutoMapper;
using Domain.Entities;

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

        public async Task<List<InventoryDto>> GetByDepotIdAsync(int depotId)
        {
            var inventories = await _unitOfWork.Inventories.FindAsync(x => x.DepotId == depotId);
            return _mapper.Map<List<InventoryDto>>(inventories);
        }

        public async Task<InventoryDto> CreateAsync(CreateInventoryDto dto)
        {
            var entity = _mapper.Map<Inventory>(dto);
            await _unitOfWork.Inventories.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<InventoryDto>(entity);
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
                .FindAsync(x => x.DepotId == depotId && !x.IsDeleted);

            var result = inventories
                .GroupBy(x => new
                {
                    StockGroupName = x.Stock.StockGroup.Name,
                    StockName = x.Stock.Name,
                    StockModel = x.Stock.Model ,
                    StockBrand = x.Stock.Brand
                })
                .Select(g => new DepotInventorySummaryDto
                {
                    StockGroupName = g.Key.StockGroupName,
                    StockName = g.Key.StockName,
                    StockModel = g.Key.StockModel,
                    StockBrand = g.Key.StockBrand,
                    TotalQuantity = g.Sum(i => i.Quantity)
                })
                .ToList();

            return result;
        }



    }
}
