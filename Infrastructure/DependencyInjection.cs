using Application.Abstractions.Repositories;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("Default");
                options.UseLazyLoadingProxies();
                options.UseSqlServer(connectionString);
            }, ServiceLifetime.Scoped);
            services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IActRepository, ActRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}