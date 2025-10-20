using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Act;

public class ActDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string FullName { get; set; }
    public string TcNo { get; set; }
    public string? CompanyName { get; set; }
}
