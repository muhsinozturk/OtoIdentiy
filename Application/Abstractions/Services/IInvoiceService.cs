using Application.DTOs.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> CreateFromWorkOrderAsync(int workOrderId);
        Task<List<InvoiceDto>> GetAllAsync();
        Task<InvoiceDto?> GetByIdAsync(int id);
    }
}
