using Domain.Common;

namespace Domain.Entities
{
    public class Inventory : BaseAuditableEntity
    {
        public int DepotId { get; set; }
        public int StockId { get; set; }

        // 🔹 Giriş veya çıkış miktarı
        public decimal Quantity { get; set; }

        // 🔹 True = Giriş, False = Çıkış
        public bool IsInput { get; set; }

        // 🔹 İlişkiler
        public virtual Depot Depot { get; set; }
        public virtual Stock Stock { get; set; }

        // 🔹 Açıklama (örnek: "Fatura #12 çıkışı", "Satın alma girişi")
        public string? Description { get; set; }

        // 🔹 İlgili fatura bağlantısı (opsiyonel)
        public int? InvoiceId { get; set; }
        public virtual Invoice? Invoice { get; set; }

        // 🔹 İleriye dönük kolaylık için iş emriyle de bağlanabilir
        public int? WorkOrderId { get; set; }
        public virtual WorkOrder? WorkOrder { get; set; }

        // 🔹 Tarih zaten BaseAuditableEntity’den geliyor:
        // CreatedAt -> Giriş veya çıkış zamanı
    }
}
