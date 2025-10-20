using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Stock;
using Application.DTOs.StockGroup;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class StockGroupService : IStockGroupService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StockGroupService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<StockGroupDto>> GetAllAsync()
        {
            var groups = await _unitOfWork.StockGroups.GetAllAsync();
            return _mapper.Map<List<StockGroupDto>>(groups);
        }

        public async Task<StockGroupDto?> GetByIdAsync(int id)
        {
            var group = await _unitOfWork.StockGroups.GetByIdAsync(id);
            return _mapper.Map<StockGroupDto>(group);
        }

        public async Task<StockGroupDto> CreateAsync(CreateStockGroupDto dto)
        {
            var entity = _mapper.Map<StockGroup>(dto);
            await _unitOfWork.StockGroups.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<StockGroupDto>(entity);
        }

        public async Task UpdateAsync(StockGroupDto dto)
        {
            var entity = await _unitOfWork.StockGroups.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new Exception("Stock group not found");

            _mapper.Map(dto, entity);
            _unitOfWork.StockGroups.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.StockGroups.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Stock group not found");

            _unitOfWork.StockGroups.Remove(entity);
            await _unitOfWork.CommitAsync();
        }
        public async Task<List<StockDto>> GetByGroupIdAsync(int groupId)
        {
            var stocks = await _unitOfWork.Stocks.FindAsync(s => s.StockGroupId == groupId);
            return _mapper.Map<List<StockDto>>(stocks);
        }
    }
}
