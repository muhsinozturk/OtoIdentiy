using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Invoice: BaseAuditableEntity
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public decimal LaborCost { get; set; }
        public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public virtual WorkOrder WorkOrder { get; set; }
    }
}
