namespace Application.DTOs.Inventory
{
    public class DepotInventorySummaryDto
    {
        public string StockGroupName { get; set; } = string.Empty;
        public string StockName { get; set; } = string.Empty;
        public string? StockModel { get; set; }
        public string? StockBrand { get; set; }

        // 🔹 Giriş/Çıkış toplamları
        public decimal TotalIn { get; set; }
        public decimal TotalOut { get; set; }

        // 🔹 Net miktar = giriş - çıkış
        public decimal NetQuantity => TotalIn - TotalOut;

        public List<DepotInventoryItemDto?> Items { get; set; } = new();
    }
}

