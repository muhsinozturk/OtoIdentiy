using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Depot;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class DepotService : IDepotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepotService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<DepotDto>> GetAllAsync()
        {
            var depots = await _unitOfWork.Depots.GetAllAsync();
            return _mapper.Map<List<DepotDto>>(depots);
        }

        public async Task<DepotDto?> GetByIdAsync(int id)
        {
            var depot = await _unitOfWork.Depots.GetByIdAsync(id);
            return _mapper.Map<DepotDto>(depot);
        }

        public async Task<DepotDto> CreateAsync(CreateDepotDto dto)
        {
            var depot = _mapper.Map<Depot>(dto);
            await _unitOfWork.Depots.AddAsync(depot);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<DepotDto>(depot);
        }

        public async Task UpdateAsync(EditDepotDto dto)
        {
            var depot = await _unitOfWork.Depots.GetByIdAsync(dto.Id);
            if (depot == null)
                throw new Exception("Depot not found");

            _mapper.Map(dto, depot);
            _unitOfWork.Depots.Update(depot);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var depot = await _unitOfWork.Depots.GetByIdAsync(id);
            if (depot == null)
                throw new Exception("Depot not found");

            _unitOfWork.Depots.Remove(depot);
            await _unitOfWork.CommitAsync();
        }
    }
}
