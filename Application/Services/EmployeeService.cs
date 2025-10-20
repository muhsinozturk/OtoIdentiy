using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Emplooye;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            var employees = await _unitOfWork.Employees.GetAllAsync();
            return _mapper.Map<List<EmployeeDto>>(employees);
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
        {
            var employee = _mapper.Map<Employee>(dto);
            await _unitOfWork.Employees.AddAsync(employee);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<EmployeeDto>(employee);
        }

        public async Task UpdateAsync(EmployeeDto dto)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(dto.Id);
            if (employee == null)
                throw new Exception("Employee not found");

            _mapper.Map(dto, employee);
            _unitOfWork.Employees.Update(employee);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                throw new Exception("Employee not found");

            _unitOfWork.Employees.Remove(employee);
            await _unitOfWork.CommitAsync();
        }
    }
}
