using Vesa.DTOs.Users;

namespace Vesa.Services.Interfaces;

public interface IUserService
{
    Task<IList<UserResponse>> GetAllAsync(string? search);
    Task<UserDetailsResponse?> GetByIdAsync(string userId);
    Task<(bool success, string? error)> RestrictAsync(string userId, string reason);
    Task<(bool success, string? error)> UnrestrictAsync(string userId);
}
