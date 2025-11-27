using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

public class AppUser : IdentityUser
{

    public string? City { get; set; }
    public string? Picture { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; }

}
