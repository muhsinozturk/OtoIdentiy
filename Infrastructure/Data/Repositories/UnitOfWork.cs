using Application.Abstractions.Repositories;
using Domain.Entities;

namespace Infrastructure.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        // Özel repositoryler
        public IWorkOrderRepository WorkOrders { get; }
        public IVehicleRepository Vehicles { get; }
        public IActRepository Acts { get; }
        public IInvoiceRepository Invoices { get; }

        // Generic repositoryler

        public IRepository<InvoiceItem> InvoiceItems { get; }
        public IRepository<Employee> Employees { get; }
        public IRepository<Depot> Depots { get; }
        public IRepository<Inventory> Inventories { get; }
        public IRepository<StockGroup> StockGroups { get; }
        public IRepository<Stock> Stocks { get; }
        public IRepository<StockPrice> StockPrices { get; }
        public IRepository<StockPriceType> StockPriceTypes { get; }
        public IRepository<Company> Companies { get; }

        public IRepository<WorkOrderPart> WorkOrderParts { get; }

        public UnitOfWork(ApplicationDbContext context,
            IWorkOrderRepository WorkOrderRepository,
            IVehicleRepository VehicleRepository,
            IActRepository ActRepository,
            IInvoiceRepository InvoiceRepository
        )

        {
            _context = context;

            // Özel repo
            WorkOrders = WorkOrderRepository;
            Vehicles = VehicleRepository;
            Acts = ActRepository;
            Invoices = InvoiceRepository;

            // Generic repo

            InvoiceItems = new Repository<InvoiceItem>(_context);
            StockGroups = new Repository<StockGroup>(_context);
            Employees = new Repository<Employee>(_context);
            Depots = new Repository<Depot>(_context);
            Inventories = new Repository<Inventory>(_context);
            Stocks = new Repository<Stock>(_context);
            StockPrices = new Repository<StockPrice>(_context);
            StockPriceTypes = new Repository<StockPriceType>(_context);
            Companies = new Repository<Company>(_context);
            WorkOrderParts = new Repository<WorkOrderPart>(_context);
        }

        public async Task<int> CommitAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
