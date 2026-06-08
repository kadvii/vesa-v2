using Vesa.DTOs.Applications;

namespace Vesa.DTOs.Users;

public class UserDetailsResponse : UserResponse
{
    public IList<ApplicationResponse> Applications { get; set; } = new List<ApplicationResponse>();
}
