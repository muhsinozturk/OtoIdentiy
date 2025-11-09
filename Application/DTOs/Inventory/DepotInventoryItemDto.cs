namespace Application.DTOs.Inventory
{
    public class DepotInventoryItemDto
    {
        public int StockId { get; set; }
        public string StockName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }

        // 🔹 Giriş/Çıkış yönü
        public bool IsInput { get; set; }

        // 🔹 Tarih
        public DateTimeOffset CreatedAt { get; set; }
    }
}
