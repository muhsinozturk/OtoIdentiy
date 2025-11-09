using Application.DTOs.Stock;

namespace Application.DTOs.Inventory
{
    public class InventoryDto
    {
        public int Id { get; set; }

        public int DepotId { get; set; }
        public string? DepotName { get; set; }

        public int StockId { get; set; }
        public string? StockName { get; set; }

        public decimal Quantity { get; set; }

        // 🔹 Giriş/Çıkış türü
        public bool IsInput { get; set; }

        // 🔹 Açıklama (örnek: "Fatura çıkışı", "Satın alma girişi")
        public string? Description { get; set; }

        // 🔹 Tarih (işlem zamanı)
        public DateTimeOffset CreatedAt { get; set; }


        // 🔹 İlişkili stok detayları
        public StockDto? StockDto { get; set; }
    }
}
