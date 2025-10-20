using Application.Abstractions.Repositories;
using Application.Abstractions.Services;
using Application.DTOs.Stock;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class StockService : IStockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StockService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<StockDto>> GetAllAsync()
        {
            var stocks = await _unitOfWork.Stocks.GetAllAsync();
            return _mapper.Map<List<StockDto>>(stocks);
        }

        public async Task<StockDto?> GetByIdAsync(int id)
        {
            var stock = await _unitOfWork.Stocks.GetByIdAsync(id);
            return _mapper.Map<StockDto>(stock);
        }

        public async Task<StockDto> CreateAsync(CreateStockDto dto)
        {
            var entity = _mapper.Map<Stock>(dto);
            await _unitOfWork.Stocks.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<StockDto>(entity);
        }

        public async Task UpdateAsync(EditStockDto dto)
        {
            var entity = await _unitOfWork.Stocks.GetByIdAsync(dto.Id);
            if (entity == null)
                throw new Exception("Stock not found");

            _mapper.Map(dto, entity);
            _unitOfWork.Stocks.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Stocks.GetByIdAsync(id);
            if (entity == null)
                throw new Exception("Stok bulunamadı.");

            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.Now;

            _unitOfWork.Stocks.Update(entity);
            await _unitOfWork.CommitAsync();
        }
        public async Task<EditStockDto?> GetByIdForEditAsync(int id)
        {
            var stock = await _unitOfWork.Stocks.GetByIdAsync(id);
            return _mapper.Map<EditStockDto>(stock);
        }
        public async Task<List<StockDto>> GetByGroupIdAsync(int groupId)
        {
            var stocks = await _unitOfWork.Stocks.FindAsync(x =>
                x.StockGroupId == groupId && !x.IsDeleted);

            return _mapper.Map<List<StockDto>>(stocks);
        }

    }
}
