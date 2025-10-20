using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Employee: BaseAuditableEntity
{
    public int Id { get; set; }
    public int CompanyId { get; set; }  
    public string FullName { get; set; }
    public string Type { get; set; }

    public virtual Company Company { get; set; }
}
