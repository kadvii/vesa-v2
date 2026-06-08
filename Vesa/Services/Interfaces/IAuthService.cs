using Vesa.DTOs.Auth;

namespace Vesa.Services.Interfaces;

public interface IAuthService
{
    Task<(bool success, AuthResponse? data, string? error)> RegisterAsync(RegisterRequest request);
    Task<(bool success, AuthResponse? data, string? error)> LoginAsync(LoginRequest request);
    Task<ProfileResponse?> GetProfileAsync(string userId);
    Task<(bool success, string? error)> UpdateProfileAsync(string userId, UpdateProfileRequest request);
    Task<(bool success, string? error)> ChangePasswordAsync(string userId, ChangePasswordRequest request);
}
