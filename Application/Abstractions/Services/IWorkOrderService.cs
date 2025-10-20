
using Application.Abstractions.Repositories;
using Application.DTOs.WorkOrder;
using Application.DTOs.WorkOrderPart;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Abstractions.Services
{
    public interface IWorkOrderService
    {
        Task<List<WorkOrderDto>> GetByVehicleIdAsync(int vehicleId);
        Task<WorkOrderDto?> GetByIdAsync(int id);
        Task<WorkOrderDto> CreateAsync(CreateWorkOrderDto dto);
        Task CloseWorkOrderAsync(int id, string description, decimal laborCost , int? employeeId);
        Task DeleteAsync(int id);

        // Parçalar
        Task AddPartAsync(CreateWorkOrderPartDto dto);
        Task RemovePartAsync(int partId);
        Task<List<WorkOrderDto>> GetAllAsync();
        Task UpdateAsync(UpdateWorkOrderDto dto);
        Task UpdateLaborCostAsync(int id, decimal laborCost);

    }
}
