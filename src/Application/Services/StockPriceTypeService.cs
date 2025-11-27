using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.StockPriceType;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class StockPriceTypeService : IStockPriceTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public StockPriceTypeService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = uow;
            _mapper = mapper;
            _http = http;
        }

        // ✔ EmployeeStyle UserId
        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 🔹 Listeleme (UserQuery => CreatedUserId + IsDeleted otomatik)
        public async Task<List<StockPriceTypeDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.StockPriceTypes
                .UserQuery(UserId)
                .ToListAsync();

            return _mapper.Map<List<StockPriceTypeDto>>(entities);
        }

        // 🔹 Tek kayıt getir
        public async Task<StockPriceTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.StockPriceTypes
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<StockPriceTypeDto>(entity);
        }

        // 🔹 Yeni fiyat tipi ekleme
        public async Task<StockPriceTypeDto> CreateAsync(CreateStockPriceTypeDto dto)
        {
            var entity = _mapper.Map<StockPriceType>(dto);

            entity.CreatedUserId = UserId;
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.StockPriceTypes.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<StockPriceTypeDto>(entity);
        }

        // 🔹 Güncelleme
        public async Task UpdateAsync(UpdateStockPriceTypeDto dto)
        {
            var entity = await _unitOfWork.StockPriceTypes
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Bu fiyat tipini güncelleme yetkiniz yok.");

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.StockPriceTypes.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Silme (soft delete)
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.StockPriceTypes
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Bu fiyat tipini silme yetkiniz yok.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;

            _unitOfWork.StockPriceTypes.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Fiyat tipine bağlı stoklar
        public async Task<List<string>> GetRelatedStocksAsync(int priceTypeId)
        {
            var stocks = await _unitOfWork.StockPrices
                .UserQuery(UserId)
                .Include(x => x.Stock)
                .Where(x => x.StockPriceTypeId == priceTypeId)
                .ToListAsync();

            return stocks
                .Select(s => s.Stock.Name)
                .Distinct()
                .ToList();
        }
    }
}
