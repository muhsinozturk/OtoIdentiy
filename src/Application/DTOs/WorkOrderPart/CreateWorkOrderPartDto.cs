using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOs.WorkOrder;
using Application.DTOs.WorkOrderPart;

public class CreateWorkOrderPartDto
{
    public int WorkOrderId { get; set; }
    public int StockId { get; set; }
    public int DepotId { get; set; }
    public int? StockPriceTypeId { get; set; }
    public decimal Quantity { get; set; }
    public decimal KdvRate { get; set; }  // Yeni eklendi
}

