using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Stock;
using Application.DTOs.StockGroup;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class StockGroupService : IStockGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public StockGroupService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _http = http;
        }

        // ✔ EmployeeService tarzında UserId property
        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // ✔ Listeleme (UserQuery otomatik CreatedUserId + IsDeleted filtresi ekler)
        public async Task<List<StockGroupDto>> GetAllAsync()
        {
            var groups = await _unitOfWork.StockGroups
                .UserQuery(UserId)
                .ToListAsync();

            return _mapper.Map<List<StockGroupDto>>(groups);
        }

        // ✔ Tek kayıt getir
        public async Task<StockGroupDto?> GetByIdAsync(int id)
        {
            var group = await _unitOfWork.StockGroups
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<StockGroupDto>(group);
        }

        // ✔ Ekleme
        public async Task<StockGroupDto> CreateAsync(CreateStockGroupDto dto)
        {
            var entity = _mapper.Map<StockGroup>(dto);

            entity.CreatedUserId = UserId;
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.StockGroups.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<StockGroupDto>(entity);
        }

        // ✔ Güncelleme
        public async Task UpdateAsync(StockGroupDto dto)
        {
            var entity = await _unitOfWork.StockGroups
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Bu stok grubuna erişim yetkiniz yok.");

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.StockGroups.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // ✔ Silme (Soft Delete istersen aynı yapıyı kullanırız)
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.StockGroups
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Bu stok grubuna erişim yetkiniz yok.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;

            _unitOfWork.StockGroups.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // ✔ Gruba ait stoklar
        public async Task<List<StockDto>> GetByGroupIdAsync(int groupId)
        {
            var stocks = await _unitOfWork.Stocks
                .UserQuery(UserId)
                .Where(s => s.StockGroupId == groupId)
                .ToListAsync();

            return _mapper.Map<List<StockDto>>(stocks);
        }
    }
}
