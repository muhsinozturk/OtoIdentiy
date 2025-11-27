using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Emplooye;

public class CreateEmployeeDto
{
    public int CompanyId { get; set; }
    public string FullName { get; set; }
    public string Type { get; set; }
}
