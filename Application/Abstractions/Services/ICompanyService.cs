using Application.DTOs.Company;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions.Services;

public interface ICompanyService
{
    Task<List<CompanyDto>> GetAllAsync();
    Task<CompanyDto?> GetByIdAsync(int id);
    Task<CompanyDto> CreateAsync(CreateCompanyDto dto);
    Task UpdateAsync(CompanyDto dto);
    Task DeleteAsync(int id);
}
