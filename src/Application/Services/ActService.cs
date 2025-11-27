using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Act;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class ActService : IActService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public ActService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _http = http;
        }

        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 📌 Tüm müşteriler (soft delete + kullanıcı filtreli)
        public async Task<List<ActDto>> GetAllAsync()
        {
            var acts = await _unitOfWork.Acts
                .UserQuery(UserId)
                .ToListAsync();

            return _mapper.Map<List<ActDto>>(acts);
        }

        // 📌 Tek müşteri (soft delete + kullanıcı filtreli)
        public async Task<ActDto?> GetByIdAsync(int id)
        {
            var act = await _unitOfWork.Acts
                .UserQuery(UserId)
                .FirstOrDefaultAsync(a => a.Id == id);

            return _mapper.Map<ActDto>(act);
        }

        // 📌 Yeni kayıt
        public async Task<ActDto> CreateAsync(CreateActDto dto)
        {
            var act = _mapper.Map<Act>(dto);

            act.CreatedUserId = UserId;
            act.CreatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.Acts.AddAsync(act);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<ActDto>(act);
        }

        // 📌 Güncelleme
        public async Task UpdateAsync(ActDto dto)
        {
            var act = await _unitOfWork.Acts
                .UserQuery(UserId)
                .FirstOrDefaultAsync(a => a.Id == dto.Id);

            if (act == null)
                throw new Exception("Kayıt bulunamadı veya yetkin yok.");

            _mapper.Map(dto, act);
            act.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Acts.Update(act);
            await _unitOfWork.CommitAsync();
        }

        // 📌 Silme (soft delete)
        public async Task DeleteAsync(int id)
        {
            var act = await _unitOfWork.Acts
                .UserQuery(UserId)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (act == null)
                throw new Exception("Kayıt bulunamadı veya yetkin yok.");

            act.IsDeleted = true;
            act.DeletedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Acts.Update(act);
            await _unitOfWork.CommitAsync();
        }

        // 📌 Müşteri + Araç detay
        public async Task<ActWithVehiclesDto?> GetWithVehiclesAsync(int id)
        {
            var act = await _unitOfWork.Acts.GetWithVehiclesAsync(id);

            if (act == null || act.CreatedUserId != UserId)
                return null;

            return _mapper.Map<ActWithVehiclesDto>(act);
        }

        // 📌 Kullanıcıya ait tüm müşteri + araçlar
        public async Task<List<ActWithVehiclesDto>> GetAllWithVehiclesAsync()
        {
            var acts = await _unitOfWork.Acts
                .UserQuery(UserId)
                .Include(a => a.Vehicles)
                .ToListAsync();

            return _mapper.Map<List<ActWithVehiclesDto>>(acts);
        }
    }
}
