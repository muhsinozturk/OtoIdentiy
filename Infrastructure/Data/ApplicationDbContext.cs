using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data;


public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<StockPriceType> StockPriceTypes { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Act> Acts { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Depot> Depots { get; set; }
    public DbSet<WorkOrder> WorkOrders { get; set; }
    public DbSet<WorkOrderPart> WorkOrderParts { get; set; }
    public DbSet<StockPrice> StockPrices { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<Invoice>Invoices{ get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<StockGroup> StockGroups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Company ilişkileri
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Company)
            .WithMany(c => c.Employees)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Depot>()
            .HasOne(d => d.Company)
            .WithMany(c => c.Depots)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Act>()
            .HasOne(a => a.Company)
            .WithMany(c => c.Acts)
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Stock>()
            .HasOne(s => s.Company)
            .WithMany(c => c.Stocks)
            .HasForeignKey(s => s.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StockPriceType>()
            .HasOne(spt => spt.Company)
            .WithMany(c => c.StockPriceTypes)
            .HasForeignKey(spt => spt.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Vehicle ilişkisi
        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Act)
            .WithMany(a => a.Vehicles)
            .HasForeignKey(v => v.ActId)
            .OnDelete(DeleteBehavior.Restrict);

        // Inventory ilişkileri
        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Depot)
            .WithMany(d => d.Inventories)
            .HasForeignKey(i => i.DepotId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Stock)
            .WithMany(s => s.Inventories)
            .HasForeignKey(i => i.StockId)
            .OnDelete(DeleteBehavior.Restrict);

        // StockPrice ilişkileri
        modelBuilder.Entity<StockPrice>()
            .HasOne(p => p.Stock)
            .WithMany(s => s.Prices)
            .HasForeignKey(p => p.StockId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<StockPrice>()
            .HasOne(p => p.StockPriceType)
            .WithMany(t => t.StockPrices)
            .HasForeignKey(p => p.StockPriceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // WorkOrder ilişkileri
        modelBuilder.Entity<WorkOrder>()
            .HasOne(w => w.Vehicle)
            .WithMany(v => v.WorkOrders)
            .HasForeignKey(w => w.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkOrder>()
            .HasOne(w => w.Employee)
            .WithMany()
            .HasForeignKey(w => w.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);

        // WorkOrderPart ilişkileri
        modelBuilder.Entity<WorkOrderPart>()
            .HasOne(p => p.WorkOrder)
            .WithMany(w => w.Parts)
            .HasForeignKey(p => p.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkOrderPart>()
            .HasOne(p => p.Stock)
            .WithMany()
            .HasForeignKey(p => p.StockId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkOrderPart>()
            .HasOne(p => p.StockPriceType)
            .WithMany()
            .HasForeignKey(p => p.StockPriceTypeId)
            .OnDelete(DeleteBehavior.SetNull);

       
    }

}