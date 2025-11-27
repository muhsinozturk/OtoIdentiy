using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Stock;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class StockService : IStockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public StockService(IUnitOfWork uow, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = uow;
            _mapper = mapper;
            _http = http;
        }

        // ✔ Diğer servislerle aynı UserId standardı
        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 🔹 Tüm stoklar
        public async Task<List<StockDto>> GetAllAsync()
        {
            var stocks = await _unitOfWork.Stocks
                .UserQuery(UserId)               // CreatedUserId + IsDeleted otomatik
                .Include(x => x.StockGroup)
                .ToListAsync();

            return _mapper.Map<List<StockDto>>(stocks);
        }

        // 🔹 Tek stok
        public async Task<StockDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Stocks
                .UserQuery(UserId)
                .Include(x => x.StockGroup)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<StockDto>(entity);
        }

        // 🔹 Düzenleme ekranı için
        public async Task<EditStockDto?> GetByIdForEditAsync(int id)
        {
            var entity = await _unitOfWork.Stocks
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<EditStockDto>(entity);
        }

        // 🔹 Yeni stok ekleme
        public async Task<StockDto> CreateAsync(CreateStockDto dto)
        {
            var entity = _mapper.Map<Stock>(dto);

            entity.CreatedUserId = UserId;
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.Stocks.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<StockDto>(entity);
        }

        // 🔹 Güncelleme
        public async Task UpdateAsync(EditStockDto dto)
        {
            var entity = await _unitOfWork.Stocks
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Bu stoğu güncelleme yetkiniz yok.");

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Stocks.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Soft delete
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Stocks
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Bu stoğu silme yetkiniz yok.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Stocks.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Belirli gruptaki stoklar
        public async Task<List<StockDto>> GetByGroupIdAsync(int groupId)
        {
            var stocks = await _unitOfWork.Stocks
                .UserQuery(UserId)
                .Where(x => x.StockGroupId == groupId)
                .ToListAsync();

            return _mapper.Map<List<StockDto>>(stocks);
        }
    }
}
