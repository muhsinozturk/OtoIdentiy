using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Company;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _http;

        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor http)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _http = http;
        }

        private string UserId =>
            _http.HttpContext!.User!.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // 🔹 Kullanıcının şirketleri (UserQuery %100 otomatik)
        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var companies = await _unitOfWork.Companies
                .UserQuery(UserId)
                .ToListAsync();

            return _mapper.Map<List<CompanyDto>>(companies);
        }

        // 🔹 Tek şirket (UserQuery %100 otomatik)
        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Companies
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<CompanyDto>(entity);
        }

        // 🔹 Yeni şirket ekleme
        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            var company = _mapper.Map<Company>(dto);

            company.CreatedUserId = UserId;
            company.CreatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.Companies.AddAsync(company);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<CompanyDto>(company);
        }

        // 🔹 Güncelleme
        public async Task UpdateAsync(CompanyDto dto)
        {
            var company = await _unitOfWork.Companies
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == dto.Id);

            if (company == null)
                throw new Exception("Kayıt bulunamadı veya yetkiniz yok.");

            _mapper.Map(dto, company);
            company.UpdatedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Companies.Update(company);
            await _unitOfWork.CommitAsync();
        }

        // 🔹 Silme (Soft Delete)
        public async Task DeleteAsync(int id)
        {
            var company = await _unitOfWork.Companies
                .UserQuery(UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (company == null)
                throw new Exception("Kayıt bulunamadı veya yetkiniz yok.");

            company.IsDeleted = true;
            company.DeletedAt = DateTimeOffset.UtcNow;

            _unitOfWork.Companies.Update(company);
            await _unitOfWork.CommitAsync();
        }
    }
}
