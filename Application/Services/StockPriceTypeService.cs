using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.StockPriceType;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class StockPriceTypeService : IStockPriceTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StockPriceTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<StockPriceTypeDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.StockPriceTypes.GetAllAsync();
            return _mapper.Map<List<StockPriceTypeDto>>(entities);
        }

        // 🔹 ID'ye göre fiyat tipi getir
        public async Task<StockPriceTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.StockPriceTypes.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
                return null;

            return _mapper.Map<StockPriceTypeDto>(entity);
        }
        public async Task<StockPriceTypeDto> CreateAsync(CreateStockPriceTypeDto dto)
        {
            var entity = _mapper.Map<StockPriceType>(dto);
            await _unitOfWork.StockPriceTypes.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<StockPriceTypeDto>(entity);
        }

        public async Task UpdateAsync(UpdateStockPriceTypeDto dto)
        {
            var entity = await _unitOfWork.StockPriceTypes.GetByIdAsync(dto.Id);
            if (entity == null) throw new Exception("Fiyat tipi bulunamadı.");

            _mapper.Map(dto, entity);
            _unitOfWork.StockPriceTypes.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.StockPriceTypes.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Fiyat tipi bulunamadı.");

            // 🔹 Soft delete işlemi
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;

            _unitOfWork.StockPriceTypes.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task<List<string>> GetRelatedStocksAsync(int priceTypeId)
        {
            var stocks = await _unitOfWork.StockPrices
                .FindAsync(x => x.StockPriceTypeId == priceTypeId && !x.IsDeleted);

            return stocks.Select(s => s.Stock.Name ).Distinct().ToList();
        }
    }
}
