using Application.DTOs.Emplooye;
using Application.DTOs.Inventory;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task UpdateAsync(EmployeeDto dto);
    Task DeleteAsync(int id);
}
