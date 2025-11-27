using Application.DTOs.WorkOrderPart;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.DTOs.WorkOrder
{
    public class WorkOrderDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string VehiclePlate { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmployeeFullName { get; set; }
        public DateTime OpenDate { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Description { get; set; }

        // 🔹 İşçilik ücreti
        public decimal LaborCost { get; set; }

        // 🔹 Fatura bilgisi
        public int? InvoiceId { get; set; }
        public bool HasInvoice { get; set; }

        // 🔹 Parçalar
        public List<WorkOrderPartDto> Parts { get; set; } = new();

        // 🔹 Hesaplamalar
        public decimal PartsTotal => Parts?.Sum(p => p.SubTotal) ?? 0;             // KDV hariç
        public decimal KdvTotal => Parts?.Sum(p => p.KdvAmount) ?? 0;              // KDV toplamı
        public decimal GrandTotal => PartsTotal + KdvTotal + LaborCost;            // Genel toplam (KDV + İşçilik dahil)
    }
}
