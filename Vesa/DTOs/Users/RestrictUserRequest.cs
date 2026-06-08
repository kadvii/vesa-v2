using System.ComponentModel.DataAnnotations;

namespace Vesa.DTOs.Users;

public class RestrictUserRequest
{
    [Required]
    public string Reason { get; set; } = string.Empty;
}
