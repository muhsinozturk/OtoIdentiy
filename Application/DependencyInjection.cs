using Application.Abstractions.Services;
using Application.Mappings;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;


namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper config
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // --- Application services ---
            services.AddScoped<IActService, ActService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IWorkOrderService, WorkOrderService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IStockPriceService, StockPriceService>();
            services.AddScoped<IStockPriceTypeService, StockPriceTypeService>();
            services.AddScoped<IDepotService, DepotService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IStockGroupService, StockGroupService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHttpClient<IExternalApiService, ExternalApiService>();

            return services;
        }
    }
}
