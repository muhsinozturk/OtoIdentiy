namespace Application.DTOs.InvoiceItem

{
    public class InvoiceItemDto
    {
        public int Id { get; set; }

        public int StockId { get; set; }
        public string StockName { get; set; }

        public int DepotId { get; set; }
        public string DepotName { get; set; }

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // 🔹 KDV oranı (%)
        public decimal KdvRate { get; set; }

        // 🔹 Ara toplam (KDV hariç)
        public decimal SubTotal => Quantity * UnitPrice;

        // 🔹 KDV tutarı
        public decimal VatAmount => SubTotal * (KdvRate / 100);

        // 🔹 KDV dahil toplam
        public decimal Total => SubTotal + VatAmount;
    }
}
