using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Depot;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class DepotService : IDepotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public DepotService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _http = http;
        }

        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 🔹 Kullanıcının tüm depoları (UserQuery otomatik CreatedUser + IsDeleted)
        public async Task<List<DepotDto>> GetAllAsync()
        {
            var depots = await _unitOfWork.Depots
                .UserQuery(UserId)
                .ToListAsync();

            return _mapper.Map<List<DepotDto>>(depots);
        }

        // 🔹 Tek depo
        public async Task<DepotDto?> GetByIdAsync(int id)
        {
            var depot = await _unitOfWork.Depots
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<DepotDto>(depot);
        }

        // 🔹 Depo oluşturma
        public async Task<DepotDto> CreateAsync(CreateDepotDto dto)
        {
            var depot = _mapper.Map<Depot>(dto);

            depot.CreatedUserId = UserId;
            depot.CreatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.Depots.AddAsync(depot);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<DepotDto>(depot);
        }

        // 🔹 Güncelleme
        public async Task UpdateAsync(EditDepotDto dto)
        {
            var depot = await _unitOfWork.Depots
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (depot == null)
                throw new Exception("Bu depoya erişim yetkiniz yok.");

            _mapper.Map(dto, depot);
            depot.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Depots.Update(depot);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Silme (Soft Delete)
        public async Task DeleteAsync(int id)
        {
            var depot = await _unitOfWork.Depots
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (depot == null)
                throw new Exception("Bu depoyu silme yetkiniz yok.");

            depot.IsDeleted = true;
            depot.DeletedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Depots.Update(depot);
            await _unitOfWork.CommitAsync();
        }
    }
}
