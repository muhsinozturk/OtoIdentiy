using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Application.Abstractions.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        // Özel repositoryler
        IWorkOrderRepository WorkOrders { get; }
        IVehicleRepository Vehicles { get; }
        IActRepository Acts { get; }
        IInvoiceRepository Invoices { get; }

        // Generic repositoryler
        IRepository<Employee> Employees { get; }
   
        IRepository<InvoiceItem> InvoiceItems { get; }
        IRepository<Depot> Depots { get; }
        IRepository<Inventory> Inventories { get; }
        IRepository<Stock> Stocks { get; }
        IRepository<StockPrice> StockPrices { get; }
        IRepository<StockPriceType> StockPriceTypes { get; }
        IRepository<Company> Companies { get; }
        IRepository<WorkOrderPart> WorkOrderParts { get; }
        IRepository<StockGroup> StockGroups { get; }

        // SaveChanges
        Task<int> CommitAsync();
    }
}
