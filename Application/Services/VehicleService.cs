using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Vehicle;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VehicleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<VehicleDto>> GetAllByActIdAsync(int actId)
        {
            var vehicles = await _unitOfWork.Vehicles.FindAsync(v => v.ActId == actId);
            return _mapper.Map<List<VehicleDto>>(vehicles);
        }

        public async Task<VehicleDto?> GetByIdAsync(int id)
        {
            var vehicle = await _unitOfWork.Vehicles.GetByIdAsync(id);
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<VehicleDto> CreateAsync(CreateVehicleDto dto)
        {
            var entity = _mapper.Map<Vehicle>(dto);
            await _unitOfWork.Vehicles.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<VehicleDto>(entity);
        }

        public async Task UpdateAsync(VehicleDto dto)
        {
            var entity = await _unitOfWork.Vehicles.GetByIdAsync(dto.Id);
            if (entity == null) throw new Exception("Vehicle not found");

            _mapper.Map(dto, entity);
            _unitOfWork.Vehicles.Update(entity);
            await _unitOfWork.CommitAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Vehicles.GetByIdAsync(id);
            if (entity == null) throw new Exception("Vehicle not found");

            _unitOfWork.Vehicles.Remove(entity);
            await _unitOfWork.CommitAsync();
        }

  
    }
}
