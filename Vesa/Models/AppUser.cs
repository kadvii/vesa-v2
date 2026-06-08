using Microsoft.AspNetCore.Identity;

namespace Vesa.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRestricted { get; set; }
    public string? RestrictionReason { get; set; }
}
