using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Emplooye;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _http = http;
        }

        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 🔹 Tüm çalışanlar (UserQuery otomatik CreatedUserId + IsDeleted)
        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employees
                .UserQuery(UserId)
                .ToListAsync();

            return _mapper.Map<List<EmployeeDto>>(employees);
        }

        // 🔹 Tek çalışan getir
        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<EmployeeDto>(employee);
        }

        // 🔹 Yeni çalışan oluştur
        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
        {
            var entity = _mapper.Map<Employee>(dto);

            entity.CreatedUserId = UserId;
            entity.CreatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.Employees.AddAsync(entity);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<EmployeeDto>(entity);
        }

        // 🔹 Güncelleme
        public async Task UpdateAsync(EmployeeDto dto)
        {
            var entity = await _unitOfWork.Employees
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (entity == null)
                throw new Exception("Bu personele erişim yetkiniz yok.");

            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Employees.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Silme (Soft Delete)
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Employees
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                throw new Exception("Bu personele erişim yetkiniz yok.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Employees.Update(entity);
            await _unitOfWork.CommitAsync();
        }
    }
}
