using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.StockPrice;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class StockPriceService : IStockPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StockPriceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<StockPriceDto>> GetByStockIdAsync(int stockId)
        {
            var prices = await _unitOfWork.StockPrices.FindAsync(x => x.StockId == stockId);
            return _mapper.Map<List<StockPriceDto>>(prices);
        }

        public async Task<StockPriceDto> CreateAsync(CreateStockPriceDto dto)
        {
            var entity = _mapper.Map<StockPrice>(dto);
            await _unitOfWork.StockPrices.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<StockPriceDto>(entity);
        }

        public async Task UpdateAsync(StockPriceDto dto)
        {
            var entity = await _unitOfWork.StockPrices.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new Exception("StockPrice not found");

            _mapper.Map(dto, entity);
            _unitOfWork.StockPrices.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.StockPrices.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("StockPrice not found");

            _unitOfWork.StockPrices.Remove(entity);
            await _unitOfWork.CommitAsync();
        }
        public async Task AddOrUpdateMultipleAsync(List<CreateStockPriceDto> priceList)
        {
            foreach (var dto in priceList)
            {
                var existing = await _unitOfWork.StockPrices
                    .SingleOrDefaultAsync(x =>
                        x.StockId == dto.StockId &&
                        x.StockPriceTypeId == dto.StockPriceTypeId &&
                        !x.IsDeleted);

                if (existing != null)
                {
                    // Mevcut fiyat varsa güncelle
                    existing.Price = dto.Price;
                    _unitOfWork.StockPrices.Update(existing);
                }
                else
                {
                    // Yoksa yeni kayıt oluştur
                    var entity = _mapper.Map<StockPrice>(dto);
                    await _unitOfWork.StockPrices.AddAsync(entity);
                }
            }

            await _unitOfWork.CommitAsync();
        }

    }
}
