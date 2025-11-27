

using Application.DTOs.InvoiceItem;

namespace Application.DTOs.Invoice
{
    public class InvoiceDto
    {
        public int Id { get; set; }

        public int WorkOrderId { get; set; }
        public DateTime Date { get; set; }

        // 🔹 Araç plakası (iş emrinden gelir)
        public string VehiclePlate { get; set; }

        // 🔹 Parçalar (fatura kalemleri)
        public List<InvoiceItemDto> Items { get; set; } = new();

        // 🔹 İşçilik tutarı (iş emrinden gelir)
        public decimal LaborCost { get; set; }

        // 🔹 Hesaplamalar
        public decimal SubTotal => Items.Sum(i => i.SubTotal);
        public decimal TotalVat => Items.Sum(i => i.VatAmount);
        public decimal Total => SubTotal + TotalVat + LaborCost;
    }
}
