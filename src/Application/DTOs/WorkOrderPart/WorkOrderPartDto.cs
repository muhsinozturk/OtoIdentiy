namespace Application.DTOs.WorkOrderPart
{
    public class WorkOrderPartDto
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public int StockId { get; set; }
        public string StockName { get; set; }
        public int? StockPriceTypeId { get; set; }
        public string? StockPriceTypeName { get; set; }
        public decimal Quantity { get; set; }

        public decimal StockPrice { get; set; }
        public decimal KdvRate { get; set; } // ✅ KDV alanı eklendi

        public int DepotId { get; set; }
        public string DepotName { get; set; }

        // 🔹 Hesaplamalar
        public decimal SubTotal => Quantity * StockPrice;
        public decimal KdvAmount => SubTotal * (KdvRate / 100);
        public decimal Total => SubTotal + KdvAmount;
    }
}
