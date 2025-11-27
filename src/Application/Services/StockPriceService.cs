using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.StockPrice;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class StockPriceService : IStockPriceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public StockPriceService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = uow;
            _mapper = mapper;
            _http = http;
        }

        // ✔ EmployeeService tarzında UserId property
        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 🔹 Bir stoğa ait fiyatları getir (UserQuery = CreatedUserId + IsDeleted)
        public async Task<List<StockPriceDto>> GetByStockIdAsync(int stockId)
        {
            var prices = await _unitOfWork.StockPrices
                .UserQuery(UserId)
                .Where(x => x.StockId == stockId)
                .Include(x => x.Stock)
                .ToListAsync();

            return _mapper.Map<List<StockPriceDto>>(prices);
        }

        // 🔹 Yeni fiyat ekleme
        public async Task<StockPriceDto> CreateAsync(CreateStockPriceDto dto)
        {
            // stoğun kullanıcının olduğundan emin ol
            var stock = await _unitOfWork.Stocks
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == dto.StockId);

            if (stock == null)
                throw new Exception("Bu stok için fiyat ekleme yetkiniz yok.");

            var entity = _mapper.Map<StockPrice>(dto);
            entity.CreatedUserId = UserId;
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.StockPrices.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<StockPriceDto>(entity);
        }

        // 🔹 Güncelleme
        public async Task UpdateAsync(StockPriceDto dto)
        {
            var entity = await _unitOfWork.StockPrices
                .UserQuery(UserId)
                .Include(x => x.Stock)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Bu fiyat kaydı size ait değil.");

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.StockPrices.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Silme (Soft Delete)
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.StockPrices
                .UserQuery(UserId)
                .Include(x => x.Stock)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Bu fiyat kaydı size ait değil.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;

            _unitOfWork.StockPrices.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Toplu ekleme/güncelleme
        public async Task AddOrUpdateMultipleAsync(List<CreateStockPriceDto> list)
        {
            foreach (var dto in list)
            {
                // stok kullanıcının mı?
                var stock = await _unitOfWork.Stocks
                    .UserQuery(UserId)
                    .FirstOrDefaultAsync(x => x.Id == dto.StockId);

                if (stock == null)
                    throw new Exception("Bu stok için işlem yapma yetkiniz yok.");

                // var mı?
                var existing = await _unitOfWork.StockPrices
                    .UserQuery(UserId)
                    .FirstOrDefaultAsync(x =>
                        x.StockId == dto.StockId &&
                        x.StockPriceTypeId == dto.StockPriceTypeId);

                if (existing != null)
                {
                    existing.Price = dto.Price;
                    existing.UpdatedAt = DateTimeOffset.UtcNow;
                    _unitOfWork.StockPrices.Update(existing);
                }
                else
                {
                    var entity = _mapper.Map<StockPrice>(dto);
                    entity.CreatedUserId = UserId;
                    entity.CreatedAt = DateTimeOffset.UtcNow;

                    await _unitOfWork.StockPrices.AddAsync(entity);
                }
            }

            await _unitOfWork.CommitAsync();
        }
    }
}
