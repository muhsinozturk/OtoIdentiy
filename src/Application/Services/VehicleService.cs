using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Vehicle;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public VehicleService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _http = http;
        }

        // ✔ EmployeeService & StockService ile aynı UserId standardı
        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 🔹 Tüm araçlar (UserQuery → CreatedUserId + IsDeleted otomatik)
        public async Task<List<VehicleDto>> GetAllAsync()
        {
            var entities = await _unitOfWork.Vehicles
                .UserQuery(UserId)
                .Include(x => x.Act)
                .ToListAsync();

            return _mapper.Map<List<VehicleDto>>(entities);
        }

        // 🔹 Tek araç
        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Vehicles
                .UserQuery(UserId)
                .Include(x => x.Act)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<VehicleDto>(entity);
        }

        // 🔹 Act altındaki araçlar
        public async Task<List<VehicleDto>> GetAllByActIdAsync(int actId)
        {
            var entities = await _unitOfWork.Vehicles
                .UserQuery(UserId)
                .Where(x => x.ActId == actId)
                .ToListAsync();

            return _mapper.Map<List<VehicleDto>>(entities);
        }

        // 🔹 Yeni araç ekleme
        public async Task<VehicleDto> CreateAsync(CreateVehicleDto dto)
        {
            var entity = _mapper.Map<Vehicle>(dto);

            entity.CreatedUserId = UserId;
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.Vehicles.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<VehicleDto>(entity);
        }

        // 🔹 Güncelleme
        public async Task UpdateAsync(VehicleDto dto)
        {
            var entity = await _unitOfWork.Vehicles
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Bu aracı güncelleme yetkiniz yok.");

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTimeOffset.UtcNow;
        

            _unitOfWork.Vehicles.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Soft Delete
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Vehicles
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Bu aracı silme yetkiniz yok.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;
            entity.CreatedUserId = UserId;

            _unitOfWork.Vehicles.Update(entity);
            await _unitOfWork.CommitAsync();
        }
    }
}
