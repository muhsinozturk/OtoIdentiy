using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Company;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<CompanyDto>> GetAllAsync()
        {
            var companies = await _unitOfWork.Companies.GetAllAsync();
            return _mapper.Map<List<CompanyDto>>(companies);
        }

        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto> CreateAsync(CreateCompanyDto dto)
        {
            var company = _mapper.Map<Company>(dto);
            await _unitOfWork.Companies.AddAsync(company);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task UpdateAsync(CompanyDto dto)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(dto.Id);
            if (company == null)
                throw new Exception("Company not found");

            _mapper.Map(dto, company);
            _unitOfWork.Companies.Update(company);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company == null)
                throw new Exception("Company not found");

            _unitOfWork.Companies.Remove(company);
            await _unitOfWork.CommitAsync();
        }

    }
}
