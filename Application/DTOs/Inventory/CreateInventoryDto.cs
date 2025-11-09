using Application.DTOs.Stock;

namespace Application.DTOs.Inventory
{
    public class CreateInventoryDto
    {
        public int DepotId { get; set; }
        public int StockId { get; set; }

        // 🔹 Stok grubu, stok listesi filtreleri için
        public int StockGroupId { get; set; }

        // 🔹 Giriş/Çıkış bilgisi
        public bool IsInput { get; set; }

        public decimal Quantity { get; set; }

        // 🔹 Açıklama (örnek: "Satın alma", "Fatura çıkışı")
        public string? Description { get; set; }

        public StockDto? StockDto { get; set; }
    }
}
