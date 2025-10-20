using Application.DTOs.Act;
using Application.DTOs.Company;
using Application.DTOs.Depot;
using Application.DTOs.Emplooye;
using Application.DTOs.Inventory;
using Application.DTOs.Invoice;
using Application.DTOs.InvoiceItem;
using Application.DTOs.Stock;
using Application.DTOs.StockGroup;
using Application.DTOs.StockPrice;
using Application.DTOs.StockPriceType;
using Application.DTOs.Vehicle;

using Application.DTOs.WorkOrder;
using Application.DTOs.WorkOrderPart;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Company ---
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<CreateCompanyDto, Company>();

            // --- Depot ---
            CreateMap<Depot, DepotDto>().ReverseMap();
            CreateMap<CreateDepotDto, Depot>();
            CreateMap<EditDepotDto, Depot>();

            // --- Employee ---
            CreateMap<Employee, EmployeeDto>()
     .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));

            CreateMap<EmployeeDto, Employee>()
                .ForMember(dest => dest.Company, opt => opt.Ignore()); // EF yanlışlıkla Company insert etmesin

            CreateMap<CreateEmployeeDto, Employee>();

            // --- Act (Cari) ---
            CreateMap<Act, ActDto>()
              .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name));
            CreateMap<ActDto, Act>();
            CreateMap<CreateActDto, Act>();

            CreateMap<Vehicle, VehicleDto>()
            .ForMember(dest => dest.ActName, opt => opt.MapFrom(src => src.Act.FullName));
            CreateMap<CreateVehicleDto, Vehicle>();
  
            CreateMap<VehicleDto, Vehicle>();
            CreateMap<Act, ActWithVehiclesDto>();
            // --- Vehicle ---
            CreateMap<Vehicle, VehicleDto>().ReverseMap();
            CreateMap<CreateVehicleDto, Vehicle>();

            // --- StockGroup ---
            CreateMap<StockGroup, StockGroupDto>().ReverseMap();
            CreateMap<CreateStockGroupDto, StockGroup>().ReverseMap();

            // --- Stock ---
            CreateMap<Stock, StockDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
                .ReverseMap();
            CreateMap<CreateStockDto, Stock>();

            CreateMap<Stock, EditStockDto>().ReverseMap()
                .ForMember(d => d.Company, opt => opt.Ignore())
                .ForMember(d => d.StockGroup, opt => opt.Ignore());

            CreateMap<StockPriceDto, StockPrice>()
                .ForMember(dest => dest.Stock, opt => opt.Ignore())
                .ForMember(dest => dest.StockPriceType, opt => opt.Ignore());
            // --- StockPrice ---
            CreateMap<StockPrice, StockPriceDto>()
                .ForMember(dest => dest.StockName, opt => opt.MapFrom(src => src.Stock.Name))
                .ForMember(dest => dest.StockPriceTypeName, opt => opt.MapFrom(src => src.StockPriceType.Name));
           
            CreateMap<CreateStockPriceDto, StockPrice>();

            // 3️⃣ StockPriceInputDto → StockPrice
            // (Toplu ekleme sırasında CreateOrUpdate işleminde kullanılabilir)
            CreateMap<StockPriceInputDto, StockPrice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StockId, opt => opt.Ignore()) // üst DTO’dan setlenecek
                .ForMember(dest => dest.StockPriceTypeId, opt => opt.MapFrom(src => src.StockPriceTypeId))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Stock, opt => opt.Ignore())
                .ForMember(dest => dest.StockPriceType, opt => opt.Ignore());

            // --- StockPriceType ---
            CreateMap<StockPriceType, StockPriceTypeDto>().ReverseMap();
            CreateMap<StockPriceType, UpdateStockPriceTypeDto>().ReverseMap();
            CreateMap<CreateStockPriceTypeDto, StockPriceType>();
            CreateMap<StockPriceType, StockPriceTypeDeleteDto>()
              .ForMember(dest => dest.RelatedStocks, opt => opt.Ignore()); // bu zaten servis tarafından doldurulacak


            // Entity -> DTO
            CreateMap<Inventory, InventoryDto>()
                .ForMember(dest => dest.DepotName, opt => opt.MapFrom(src => src.Depot.Name))
                .ForMember(dest => dest.StockName, opt => opt.MapFrom(src => src.Stock.Name))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.StockDto, opt => opt.MapFrom(src => src.Stock));

            // DTO -> Entity
            CreateMap<InventoryDto, Inventory>()
                .ForMember(dest => dest.Depot, opt => opt.Ignore())
                .ForMember(dest => dest.Stock, opt => opt.Ignore());

            // CreateInventoryDto -> Entity
            CreateMap<CreateInventoryDto, Inventory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id otomatik artar
                .ForMember(dest => dest.Depot, opt => opt.Ignore())
                .ForMember(dest => dest.Stock, opt => opt.Ignore());

            // Entity -> CreateInventoryDto (örnek: form doldururken)
            CreateMap<Inventory, CreateInventoryDto>()
                .ForMember(dest => dest.StockDto, opt => opt.MapFrom(src => src.Stock));

            CreateMap<IGrouping<StockGroup, Inventory>, DepotInventorySummaryDto>()
        .ForMember(dest => dest.StockGroupName, opt => opt.MapFrom(src => src.Key.Name))
        .ForMember(dest => dest.TotalQuantity, opt => opt.MapFrom(src => src.Sum(x => x.Quantity)))
        .ForMember(dest => dest.StockName, opt => opt.MapFrom(src => src.First().Stock.Name))
        .ForMember(dest => dest.StockModel, opt => opt.MapFrom(src => src.First().Stock.Model))
        .ForMember(dest => dest.StockBrand, opt => opt.MapFrom(src => src.First().Stock.Brand)
        );


            // --- WorkOrder ---
            CreateMap<CreateWorkOrderDto, WorkOrder>();

            // WorkOrder -> WorkOrderDto
            CreateMap<WorkOrder, WorkOrderDto>()
                .ForMember(dest => dest.VehiclePlate, opt => opt.MapFrom(src => src.Vehicle.Plate))
                .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null))
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice != null ? src.Invoice.Id : (int?)null))
                .ForMember(dest => dest.HasInvoice, opt => opt.MapFrom(src => src.Invoice != null))
                .ForMember(dest => dest.Parts, opt => opt.MapFrom(src => src.Parts)); // Parçaları map’le

            // WorkOrderPart
            // CreateWorkOrderPartDto -> WorkOrderPart
            CreateMap<WorkOrderPart, WorkOrderPartDto>()
      .ForMember(dest => dest.StockName, opt => opt.MapFrom(src => src.Stock.Name))
      .ForMember(dest => dest.DepotName, opt => opt.MapFrom(src => src.Depot.Name))
      .ForMember(dest => dest.StockPriceTypeName, opt => opt.MapFrom(src => src.StockPriceType != null ? src.StockPriceType.Name : null))
      .ForMember(dest => dest.KdvRate, opt => opt.MapFrom(src => src.KdvRate)); // ✅ Ekledik

            CreateMap<CreateWorkOrderPartDto, WorkOrderPart>()
      .ForMember(dest => dest.KdvRate, opt => opt.MapFrom(src => src.KdvRate));


            CreateMap<Invoice, InvoiceDto>()
          .ForMember(dest => dest.VehiclePlate, opt => opt.MapFrom(src => src.WorkOrder.Vehicle.Plate))
          .ReverseMap();

            CreateMap<InvoiceItem, InvoiceItemDto>().ReverseMap();


            CreateMap<UpdateWorkOrderDto, WorkOrder>()
    .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id zaten değişmeyecek
    .ForMember(dest => dest.OpenDate, opt => opt.Ignore()) // Açılış tarihi korunacak
    .ForMember(dest => dest.CloseDate, opt => opt.Ignore()) // Serviste set edilecek
    .ForMember(dest => dest.Vehicle, opt => opt.Ignore())   // Navigation property’leri dokunma
    .ForMember(dest => dest.Employee, opt => opt.Ignore()); // Navigation proper
        }
    }
}
