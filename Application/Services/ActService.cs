using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Act;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class ActService : IActService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ActService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ActDto>> GetAllAsync()
        {
            var acts = await _unitOfWork.Acts.GetAllAsync();
            return _mapper.Map<List<ActDto>>(acts);
        }

        public async Task<ActDto?> GetByIdAsync(int id)
        {
            var act = await _unitOfWork.Acts.GetByIdAsync(id);
            return _mapper.Map<ActDto>(act);
        }

        public async Task<ActDto> CreateAsync(CreateActDto dto)
        {
            var act = _mapper.Map<Act>(dto);
            await _unitOfWork.Acts.AddAsync(act);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ActDto>(act);
        }

        public async Task UpdateAsync(ActDto dto)
        {
            var act = await _unitOfWork.Acts.GetByIdAsync(dto.Id);
            if (act == null) throw new Exception("Act not found");

            _mapper.Map(dto, act);
            _unitOfWork.Acts.Update(act);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var act = await _unitOfWork.Acts.GetByIdAsync(id);
            if (act == null) throw new Exception("Act not found");

            _unitOfWork.Acts.Remove(act);
            await _unitOfWork.CommitAsync();
        }

        public async Task<ActWithVehiclesDto?> GetWithVehiclesAsync(int id)
        {
            var act = await _unitOfWork.Acts.GetWithVehiclesAsync(id);
            return _mapper.Map<ActWithVehiclesDto>(act);
        }
        public async Task<List<ActWithVehiclesDto>> GetAllWithVehiclesAsync()
        {
            var acts = await _unitOfWork.Acts.GetAllWithVehiclesAsync();
            return _mapper.Map<List<ActWithVehiclesDto>>(acts);
        }


    }
}
