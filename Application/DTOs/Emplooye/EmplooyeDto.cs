using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Emplooye;

public class EmployeeDto
{
    public int CompanyId { get; set; }
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Type { get; set; } // teknisyen, danışman, vs
    public string? CompanyName { get; set; }
}
