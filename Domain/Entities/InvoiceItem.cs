using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class InvoiceItem: BaseAuditableEntity
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }

        public int StockId { get; set; }
        public string StockName { get; set; }

        public int DepotId { get; set; }
        public string DepotName { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // 🔹 KDV alanları
        public decimal KdvRate { get; set; } // % oran
        public decimal KdvAmount => (Quantity * UnitPrice) * (KdvRate / 100); // Hesaplanan tutar

        // 🔹 KDV dahil toplam
        public decimal Total => (Quantity * UnitPrice) + KdvAmount;

        // 🔹 Navigasyon
        public virtual Invoice Invoice { get; set; }
    }
}
